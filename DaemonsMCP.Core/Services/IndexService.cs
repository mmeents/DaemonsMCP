using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DaemonsMCP.Core.Services {
  
  public class IndexService : IIndexService, IDisposable {
    private volatile bool _isDisposed = false;
    private readonly IAppConfig _appConfig;
    private readonly IValidationService _validationService;
    private readonly ISecurityService _securityService;
    private readonly IIndexRepository _indexRepository;
    private Timer? _processTimer;    
    private readonly Lock _timerLock = new();
    private readonly List<IndexProjectItem> _projectIndexModels = new();
    private readonly ILogger<IndexService> _logger;
    private bool _isProcessing = false;
    private bool _isEnabled = true;

    public IndexService (
      ILoggerFactory loggerFactory,
      IAppConfig appConfig, 
      IValidationService validationService, 
      ISecurityService securityService,
      IIndexRepository indexRepository) {
        _logger = loggerFactory?.CreateLogger<IndexService>() ?? throw new ArgumentNullException(nameof(loggerFactory), "LoggerFactory cannot be null");
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig), "AppConfig cannot be null");
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService), "ValidationService cannot be null");
        _securityService = securityService;
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository), "IndexRepository cannot be null");

        if (_appConfig.Projects == null || !_appConfig.Projects.Any()) {
          throw new InvalidOperationException("No projects configured in AppConfig");
        }
        _projectIndexModels = _appConfig.Projects.Select(p => new IndexProjectItem() { 
            Name = p.Value.Name,
            Path = p.Value.Path,
            ProjectIndex = _indexRepository.GetProjectIndex(p.Value.Name) ?? new ProjectIndexModel(loggerFactory, p.Value, _validationService)
        }).ToList();
        foreach (var project in _projectIndexModels) {
          if (project.ProjectIndex == null) {
            throw new InvalidOperationException($"Project index not found for project: {project.Name}");
          }
          project.ProjectIndex.IndexService = this;          
        }
    }

    public IndexStatusResult GetIndexStatus() {
      var statusList = new IndexStatusResult();
      statusList.Enabled = _isEnabled;
      foreach (var project in _projectIndexModels) {
        if (project.ProjectIndex == null) continue;
        var status = new IndexProjectResult {
          ProjectName = project.Name,          
          FileCount = project.ProjectIndex.GetFileCount(),
          ClassCount = project.ProjectIndex.GetClassCount(),
          MethodCount = project.ProjectIndex.GetMethodCount(),          
        };
        statusList.Projects.Add(status);
      }
      return statusList;
    }

    public bool Enabled { 
        get{ return _isEnabled; } 
        set{
          _isEnabled = value;
          if (_isEnabled) {            
            StartTimer();
          } else {
            StopTimer();
          }
          foreach (var project in _projectIndexModels) {
            project.ProjectIndex.SetWatchServiceEnabled(_isEnabled);
          }
        } 
    } 

    public void StartTimer() {
      if (_isEnabled == false) {
        _logger.LogDebug("⚠️ Indexing is disabled, not starting timer");
        return;
      }
      lock (_timerLock) {
        if (_processTimer != null) {
          _logger.LogDebug("⚠️ Timer already running, skipping start");
          return;
        }

        _processTimer = new Timer(async _ => {
          await ProcessQueueWithTimerControl().ConfigureAwait(false);
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(Cx.IndexTimerIntervalSec));

        _logger.LogDebug($"▶️ Queue processing timer started ({Cx.IndexTimerIntervalSec} second interval)");
      }
    }

    public void StopTimer() {
      lock (_timerLock) {
        if (_processTimer == null) {
          _logger.LogDebug("⚠️ Timer already stopped, skipping stop");
          return;
        }

        _processTimer.Dispose();
        _processTimer = null;
        _logger.LogDebug("⏹️ Queue processing timer stopped");
      }
    }

    private async Task ProcessQueueWithTimerControl() {
      if (_isDisposed) return;
      // Stop the timer before processing
      StopTimer();
      var wasError = false;
      try {
        _logger.LogDebug("⏸️ Timer paused - starting queue processing");
        foreach (var project in _projectIndexModels) {
          if ( project != null && project.ProjectIndex != null 
            && project.ProjectIndex.ChangeQueue != null
            && project.ProjectIndex.ChangeQueue.Count > 0) {
            await ProcessQueueBatchAsync(project).ConfigureAwait(false);
          }
        }
        _logger.LogDebug("✅ Queue processing completed");
      } catch (Exception ex) {
        _logger.LogError(ex, "❌ Error processing index queue: {Message}", ex.Message);
        wasError = true;
      } finally {
        // Restart the timer after processing
        if (!_isDisposed) {
          var shouldRestart = !wasError;
          foreach (var project in _projectIndexModels) {
            if (project != null && project.ProjectIndex != null
              && project.ProjectIndex.ChangeQueue != null
              && project.ProjectIndex.ChangeQueue.Count > 0) {
              shouldRestart = true; // If any project has changes, restart the timer
            }
          }
          if (shouldRestart && _isEnabled) {
            _logger.LogDebug("▶️ Restarting timer after queue processing");
            StartTimer();
          }
        }
      } // End of finally block
    }

    /// <summary>
    /// RebuildIndexAsync builds the index for each configured project by scanning all .cs
    /// files, extracting classes, methods, properties, and events.  Saves to the index repository.
    /// Which saves to the .daemons folder in the project path. IsResync when true skips 
    /// if size and last modified are same.  Calls ProcessFileAsync for each index file.
    /// </summary>
    /// <param name="IsResync"></param>
    /// <returns></returns>
    public async Task<OperationResult> RebuildIndexAsync(bool IsResync = false) {
      try {
        string indexData = string.Empty;
        
        foreach (var project in _projectIndexModels) {
          List<string> projectFiles = new();
          var fullPath = Path.GetFullPath( project.Path);
          var files = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories)
             .Where(file => _securityService.IsFileAllowed(file))
             .ToHashSet(StringComparer.OrdinalIgnoreCase);          

          projectFiles.AddRange(files);
          var projectIndex = project.ProjectIndex;
          if (projectIndex == null) {
            throw new InvalidOperationException($"Project index not found for project: {project.Name}");
          }

          try {  
            // Phase 2: Cleanup - Remove files that no longer exist
            var indexedFiles = projectIndex.GetAllFileItems();
            var filesToRemove = indexedFiles.Where(f => !files.Contains(f.FilePathName)).ToList();
            if (filesToRemove.Count > 0) {
              projectIndex.RemoveFileAndAllRelated(filesToRemove);
            }          

            foreach (var filePath in projectFiles) {               
              projectIndex = await ProcessFileAsync(projectIndex, filePath, IsResync).ConfigureAwait(false);
            }

          } catch (Exception ex) {
            _logger.LogError(ex, $"Error processing files for project {project.Name}: {ex.Message}");
          } finally {
            projectIndex.WriteIndex();  // saves once per project
          }
          
        }
        var status = GetIndexStatus();        

        return OperationResult.CreateSuccess("GetIndex", "Index rebuilt successfully", status);
      } catch (Exception ex) {
        return OperationResult.CreateFailure("GetIndex", $"Index rebuild error: {ex.Message}", ex);
      }
    }

    public async Task<ProjectIndexModel> ProcessFileAsync(ProjectIndexModel aProjectIndexModel, string filePath, bool IsResync) {

      var fileInfo = new FileInfo(filePath);
      var indexFileItem = aProjectIndexModel.GetFileItemByPathName(filePath);
      if (indexFileItem == null) {
        indexFileItem = new IndexFileItem() {
          FilePathName = filePath,
          Size = fileInfo.Length,
          Modified = fileInfo.LastWriteTimeUtc,
        };
        aProjectIndexModel.InsertFileItem(indexFileItem);
      } else {
        if (IsResync &&
          indexFileItem.Size == fileInfo.Length &&
          Math.Abs((indexFileItem.Modified - fileInfo.LastWriteTimeUtc).TotalSeconds) < 1) {
          return aProjectIndexModel; // File unchanged, skip processing
        }
        indexFileItem.Size = fileInfo.Length;
        indexFileItem.Modified = fileInfo.LastWriteTimeUtc;
        aProjectIndexModel.UpdateFileItem(indexFileItem);
      }
      aProjectIndexModel.ClearClassIndex(indexFileItem); // Clear existing class index for this file

      // Parse the C# file to extract classes, methods, properties
      var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);
      if (string.IsNullOrWhiteSpace(fileContent)) {
        return aProjectIndexModel; // Skip empty files
      }
      var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
      var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
      var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
      foreach (var namespaceDecl in namespaceDeclarations) {
        string namespaceName = namespaceDecl.Name.ToString();
        var classes = namespaceDecl.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach (var classDecl in classes) {
          var className = classDecl.Identifier.Text;
          int startLine = classDecl.GetLocation().GetLineSpan().StartLinePosition.Line;
          int endLine = classDecl.GetLocation().GetLineSpan().EndLinePosition.Line;
          IndexClassItem indexClassItem = new IndexClassItem() {
            FileItemId = indexFileItem.Id,
            FileName = filePath,
            Namespace = namespaceName,
            Name = className,
            LineStart = startLine,
            LineEnd = endLine
          };
          indexClassItem = aProjectIndexModel.InsertClassItem(indexClassItem);

          var methods = classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Select(m => new IndexMethodItem() {
              Name = m.Identifier.Text,
              ClassId = indexClassItem.Id,
              ReturnType = m.ReturnType.ToString(),
              Parameters = JsonSerializer.Serialize(m.ParameterList.Parameters.Select(p => new { Type = p.Type.ToString(), Name = p.Identifier.Text }).ToList()),
              LineStart = m.GetLocation().GetLineSpan().StartLinePosition.Line,
              LineEnd = m.GetLocation().GetLineSpan().EndLinePosition.Line 
            }).ToList();

          foreach (var method in methods) {
            aProjectIndexModel.InsertMethodItem(method);
          }
                    
          var properties = classDecl.DescendantNodes().OfType<PropertyDeclarationSyntax>()
            .Select(p => new IndexPropertyItem() {
              Name = p.Identifier.Text,
              Type = p.Type.ToString(),
              ClassId = indexClassItem.Id,
              LineStart = p.GetLocation().GetLineSpan().StartLinePosition.Line,
              LineEnd = p.GetLocation().GetLineSpan().EndLinePosition.Line,
              HasGetter = p.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)) ?? false,
              HasSetter = p.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) ?? false
            });

          foreach (var property in properties) {
              aProjectIndexModel.InsertPropertyItem(property);
          }

          var events = classDecl.DescendantNodes().OfType<EventDeclarationSyntax>()
           .Select(e => new IndexEventItem() {
             Name = e.Identifier.Text,
             Type = e.Type.ToString(),
             ClassId = indexClassItem.Id,
             LineStart = e.GetLocation().GetLineSpan().StartLinePosition.Line,
             LineEnd = e.GetLocation().GetLineSpan().EndLinePosition.Line
           });

          foreach (var eventItem in events) {
            aProjectIndexModel.InsertEventItem(eventItem);
          }

        } // End of classes in namespace
      }  // End of namespace declarations

      return aProjectIndexModel;
    }


    private async Task ProcessQueueBatchAsync(IndexProjectItem project) {

      var batch = new List<FileChangeItem>();
      var maxBatchSize = 50; // Process up to 50 changes at once

      // Drain the queue into a batch
      if (project.ProjectIndex == null) return;
      if (project.ProjectIndex.ChangeQueue == null) return;
      while (batch.Count < maxBatchSize &&  project.ProjectIndex.ChangeQueue.TryDequeue(out var item)) {
        batch.Add(item);
      }

      if (batch.Count == 0) return;

      // Deduplicate - keep only the latest change per file
      var latestChanges = batch
          .GroupBy(item => item.FilePath, StringComparer.OrdinalIgnoreCase)
          .Select(group => group.OrderByDescending(item => item.Timestamp).First())
          .ToList();

      if (Cx.IsDebug) _logger.LogDebug($"Processing {latestChanges.Count} file changes for {project.Name}");

      foreach (var change in latestChanges) {
        await ProcessSingleChangeAsync(project, change);
      }

      // Save changes after processing batch
      project.ProjectIndex.WriteIndex();
      
     
    }

    private async Task ProcessSingleChangeAsync(IndexProjectItem project, FileChangeItem change) {
      try {
        switch (change.ChangeType) {
          case WatcherChangeTypes.Deleted:
            await HandleFileDeletedAsync(project, change.FilePath);
            break;

          case WatcherChangeTypes.Created:
          case WatcherChangeTypes.Changed:
            await HandleFileChangedAsync(project, change.FilePath);
            break;
        }
      } catch (Exception ex) {
        _logger.LogDebug($"Error processing {change.ChangeType} for {change.FilePath}: {ex.Message}");
      }
    }

    private async Task HandleFileDeletedAsync(IndexProjectItem project, string filePath) {
      var fileItem = project.ProjectIndex.GetFileItemByPathName(filePath);
      if (fileItem != null) {
        List<IndexFileItem> filesToRemove = new() { fileItem };
        project.ProjectIndex.RemoveFileAndAllRelated(filesToRemove);
        if (Cx.IsDebug) _logger.LogDebug($"Removed from index: {filePath}");
      }
    }

    private async Task HandleFileChangedAsync(IndexProjectItem project, string filePath) {
      // Use your existing security service check
      if (!_securityService?.IsFileAllowed(filePath) ?? false) {
        return;
      }

      if (!File.Exists(filePath)) {
        await HandleFileDeletedAsync(project, filePath);
        return;
      }

      try {
        var fileInfo = new FileInfo(filePath);
        var existingFileItem = project.ProjectIndex.GetFileItemByPathName(filePath);

        // Check if file actually changed (avoid processing temp file writes)
        if (existingFileItem != null &&
            existingFileItem.Size == fileInfo.Length &&
            Math.Abs((existingFileItem.Modified - fileInfo.LastWriteTimeUtc).TotalSeconds) < 1) {
          return; // No meaningful change
        }

        await ProcessFileAsync(project.ProjectIndex, filePath, false);

        if (Cx.IsDebug) _logger.LogDebug($"Re-indexed: {Path.GetFileName(filePath)}");

      } catch (Exception ex) {
        _logger.LogDebug($"Error indexing {filePath}: {ex.Message}");
      }
    }

    public void Dispose() {      
      if (_isDisposed) return;
      _isDisposed = true;         
      _processTimer?.Dispose();
      foreach (var project in _projectIndexModels) {
        project?.ProjectIndex?.Dispose();
      }
      _projectIndexModels.Clear();
      _logger.LogDebug($"Index Service Disposed");
    }
  }
}
