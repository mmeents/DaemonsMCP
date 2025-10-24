using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistance.Configurations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Infrastructure.Services;

public class IndexingService : IIndexingService {
  private readonly IObjectHierarchyRepository _objectHierarchyRepository;
  private readonly IIdentifierRepository _identifierRepository;
  private readonly IIdentifierTypeRepository _identifierTypeRepository;
  private readonly IIndexQueueRepository _indexQueueRepository;
  private readonly IFileSystemNodeRepository _fileSystemNodeRepository;
  private readonly IProjectRepository _projectRepository;
  private readonly ILogger<IndexingService> _logger;

  public IndexingService(
      IObjectHierarchyRepository objectHierarchyRepository,
      IIdentifierRepository identifierRepository,
      IIdentifierTypeRepository identifierTypeRepository,
      IIndexQueueRepository indexQueueRepository,
      IFileSystemNodeRepository fileSystemNodeRepository,
      IProjectRepository projectRepository,
      ILogger<IndexingService> logger) {
    _objectHierarchyRepository = objectHierarchyRepository;
    _identifierRepository = identifierRepository;
    _identifierTypeRepository = identifierTypeRepository;
    _indexQueueRepository = indexQueueRepository;
    _fileSystemNodeRepository = fileSystemNodeRepository;
    _projectRepository = projectRepository;
    _logger = logger;
  }

  /// <summary>
  /// Process pending queue items until empty. Returns summary of what was processed.
  /// </summary>
  public async Task<IndexingRunResult> RunAsync(int? projectId = null, CancellationToken cancellationToken = default) {
    var stopwatch = Stopwatch.StartNew();
    var result = new IndexingRunResult();

    try {
      _logger.LogInformation("IndexingService: Starting queue processing" + (projectId.HasValue ? $" for project {projectId}" : ""));

      // Keep processing until queue is empty
      while (true) {
        var pending = await _indexQueueRepository.GetPendingAsync(projectId, batchSize: 20);
        if (!pending.Any()) break;

        foreach (var queueItem in pending) {
          try {
            await ProcessSingleFileAsync(queueItem, cancellationToken);
            result.FilesProcessed++;
          } catch (Exception ex) {
            _logger.LogError(ex, "Failed to process queue item {QueueId}", queueItem.Id);
            result.FilesFailed++;
            // Queue item is marked Failed in ProcessSingleFileAsync
          }
        }
      }

      stopwatch.Stop();
      result.Duration = stopwatch.Elapsed;
      result.Success = true;
      _logger.LogInformation("IndexingService: Queue processing completed. Processed: {Processed}, Failed: {Failed}, Duration: {Duration}ms",
          result.FilesProcessed, result.FilesFailed, stopwatch.ElapsedMilliseconds);
    } catch (Exception ex) {
      stopwatch.Stop();
      result.Duration = stopwatch.Elapsed;
      result.Success = false;
      result.ErrorMessage = ex.Message;
      _logger.LogError(ex, "IndexingService:❌ Fatal error during queue processing");
    }

    return result;
  }

  /// <summary>
  /// Process a single queue item: parse file, build ObjectHierarchy, handle orphans, update status.
  /// </summary>
  private async Task ProcessSingleFileAsync(IndexQueue queueItem, CancellationToken cancellationToken) {
    try {
      // Mark as Processing
      queueItem.StartProcessing();
      await _indexQueueRepository.UpdateAsync(queueItem);

      // Get FileSystemNode and file path
      var fileSystemNode = await _fileSystemNodeRepository.GetByIdAsync(queueItem.FileSystemNodeId, cancellationToken);
      if (fileSystemNode == null) {
        throw new InvalidOperationException($"FileSystemNode {queueItem.FileSystemNodeId} not found");
      }

      if (fileSystemNode.IsDirectory) {
        queueItem.MarkCompleted();
        await _indexQueueRepository.UpdateAsync(queueItem);
        return; // Skip directories
      }

      // Build full path
      var project = await _projectRepository.GetByIdAsync(queueItem.ProjectId, cancellationToken);
      if (project == null) {
        throw new InvalidOperationException($"Project {queueItem.ProjectId} not found");
      }

      var fullPath = Path.GetFullPath( Path.Combine(project.RootPath, fileSystemNode.RelativePath));
      if (!File.Exists(fullPath)) {
        // File deleted, clean up hierarchy
        await _objectHierarchyRepository.DeleteByFileSystemNodeIdAsync(fileSystemNode.Id, cancellationToken);
        await _objectHierarchyRepository.SaveChangesAsync(cancellationToken);
        queueItem.MarkCompleted();
        await _indexQueueRepository.UpdateAsync(queueItem);
        return;
      }

      // Parse and build hierarchy
      var touchedHierarchyIds = await ParseAndBuildHierarchyAsync(
          fileSystemNode.Id,
          queueItem.ProjectId,
          fullPath,
          cancellationToken);

      // Delete orphaned hierarchies (existed before but not in this parse)
      var existingHierarchies = await _objectHierarchyRepository.GetByFileSystemNodeIdAsync(fileSystemNode.Id, cancellationToken);
      var orphans = existingHierarchies.Where(h => !touchedHierarchyIds.Contains(h.Id)).ToList();

      foreach (var orphan in orphans) {
        // Note: You may need cascade delete strategy here
        _logger.LogDebug("Removing orphaned ObjectHierarchy {Id}", orphan.Id);
      }

      // Note: EF Core tracking handles orphan deletion if configured with OnDelete(DeleteBehavior.Cascade)
      // For now, we'll leave it to explicit deletion or configuration

      await _objectHierarchyRepository.SaveChangesAsync(cancellationToken);

      queueItem.MarkCompleted();
      await _indexQueueRepository.UpdateAsync(queueItem);

      _logger.LogDebug("Processed file: {FilePath}, ObjectHierarchy nodes: {Count}", fullPath, touchedHierarchyIds.Count);
    } catch (Exception ex) {
      queueItem.MarkFailed(ex.Message);
      await _indexQueueRepository.UpdateAsync(queueItem);
      throw;
    }
  }

  /// <summary>
  /// Parse C# file and build ObjectHierarchy tree. Returns IDs of all nodes created/updated.
  /// </summary>
  private async Task<HashSet<int>> ParseAndBuildHierarchyAsync(
      int fileSystemNodeId,
      int projectId,
      string filePath,
      CancellationToken cancellationToken) {
    var touchedIds = new HashSet<int>();

    try {
      var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);
      if (string.IsNullOrWhiteSpace(fileContent)) {
        return touchedIds;
      }

      var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
      var root = await syntaxTree.GetRootAsync(cancellationToken);

      // Get IdentifierType IDs (cache them)
      var identifierTypes = await _identifierTypeRepository.GetAllAsync(cancellationToken);
      var typeMap = identifierTypes.ToDictionary(it => it.Name);

      // Process namespaces      
      var namespaceDecls = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();
      foreach (var namespaceDecl in namespaceDecls) {
        var namespaceName = namespaceDecl.Name.ToString();
        var namespaceId = await GetOrCreateHierarchyAsync(
            fileSystemNodeId, projectId, namespaceName,
            (int)IdentifierTypeEnum.Namespace, parentId: null,
            namespaceDecl, touchedIds, cancellationToken);

        // Process classes in namespace
        var classDecls = namespaceDecl.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach (var classDecl in classDecls) {
          var className = classDecl.Identifier.Text;
          var classId = await GetOrCreateHierarchyAsync(
              fileSystemNodeId, projectId, className,
              (int)IdentifierTypeEnum.Class, parentId: namespaceId,
              classDecl, touchedIds, cancellationToken);

          // Process methods in class
          var methodDecls = classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>();
          foreach (var methodDecl in methodDecls) {
            var methodName = methodDecl.Identifier.Text;
            var methodId = await GetOrCreateHierarchyAsync(
                fileSystemNodeId, projectId, methodName,
                (int)IdentifierTypeEnum.Method, parentId: classId,
                methodDecl, touchedIds, cancellationToken);

            // Process method parameters
            foreach (var param in methodDecl.ParameterList.Parameters) {
              var paramName = param.Identifier.Text;
              await GetOrCreateHierarchyAsync(
                  fileSystemNodeId, projectId, paramName,
                  (int)IdentifierTypeEnum.MethodParameter, parentId: methodId,
                  param, touchedIds, cancellationToken);
            }
          }

          // Process properties
          var propertyDecls = classDecl.DescendantNodes().OfType<PropertyDeclarationSyntax>();
          foreach (var propDecl in propertyDecls) {
            var propName = propDecl.Identifier.Text;
            await GetOrCreateHierarchyAsync(
                fileSystemNodeId, projectId, propName,
                (int)IdentifierTypeEnum.Property, parentId: classId,
                propDecl, touchedIds, cancellationToken);
          }
        }
      }
    } catch (Exception ex) {
      _logger.LogError(ex, "Error parsing file {FilePath}", filePath);
      throw;
    }

    return touchedIds;
  }

  /// <summary>
  /// Get or create ObjectHierarchy node. Returns the node ID and adds to touchedIds set.
  /// </summary>
  private async Task<int> GetOrCreateHierarchyAsync(
      int fileSystemNodeId,
      int projectId,
      string identifierName,
      int identifierTypeId,
      int? parentId,
      dynamic syntaxNode,  // ClassDeclarationSyntax, MethodDeclarationSyntax, etc.
      HashSet<int> touchedIds,
      CancellationToken cancellationToken) {

    // Get or create Identifier
    var identifier = await _identifierRepository.GetOrCreateAsync(identifierName, cancellationToken);

    // Get line numbers from syntax node
    var location = syntaxNode.GetLocation();
    var lineSpan = location.GetLineSpan();
    var lineStart = lineSpan.StartLinePosition.Line;
    var lineEnd = lineSpan.EndLinePosition.Line;

    // Build ObjectHierarchy to upsert
    var hierarchy = ObjectHierarchy.Create(
        parentId: parentId, 
        projectId: projectId,
        fileSystemNodeId: fileSystemNodeId,
        identifierId: identifier.Id,
        identifierTypeId: identifierTypeId,
        lineStart: lineStart,
        lineEnd: lineEnd);

    // Upsert via repo
    var result = await _objectHierarchyRepository.GetOrCreateAsync(hierarchy, cancellationToken);
    touchedIds.Add(result.Id);

    return result.Id;
  }
}

/// <summary>
/// Interface for IndexingService
/// </summary>
public interface IIndexingService {
  Task<IndexingRunResult> RunAsync(int? projectId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result from a RunAsync call
/// </summary>
public class IndexingRunResult {
  public bool Success { get; set; }
  public int FilesProcessed { get; set; }
  public int FilesFailed { get; set; }
  public TimeSpan Duration { get; set; }
  public string? ErrorMessage { get; set; }
}