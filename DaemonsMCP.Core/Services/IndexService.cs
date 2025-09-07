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
    private Timer? _scheduledCmdTimer;
    private readonly Lock _processTimerLock = new();
    private readonly Lock _scheduledCmdLock = new();
    private readonly List<IndexProjectItem> _projectIndexModels = new();
    private readonly ILogger<IndexService> _logger;
    private bool _isProcessing = false;
    private bool _isEnabled = false;

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
            ProjectIndex = _indexRepository.GetProjectIndex(p.Value.Name) ?? new ProjectIndexModel(loggerFactory, p.Value, _validationService, securityService)
        }).ToList();
        foreach (var project in _projectIndexModels) {
          if (project.ProjectIndex == null) {
            throw new InvalidOperationException($"Project index not found for project: {project.Name}");
          }
          project.ProjectIndex.IndexService = this;
          
        }
        Enabled = true;
    }

    public async Task<IndexStatusResult> GetIndexStatus() {
      return await Task.Run(() => {
        var statusList = new IndexStatusResult();
        statusList.Enabled = _isEnabled;
        foreach (var project in _projectIndexModels) {
          if (project.ProjectIndex == null) continue;
          var status = new IndexProjectResult {
            ProjectName = project.Name,
            FileCount = project.ProjectIndex.GetFileCount(),
            ClassCount = project.ProjectIndex.GetClassCount(),
            MethodCount = project.ProjectIndex.GetMethodCount(),
            IndexQueuedCount = project.ProjectIndex.ChangeQueue?.Count ?? 0
          };
          statusList.Projects.Add(status);
        }
        return statusList;
      }).ConfigureAwait(false);
      
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
      lock (_processTimerLock) {
        if (_processTimer != null) {
          _logger.LogDebug("⚠️ Timer already running, skipping start");
          return;
        }

        _processTimer = new Timer(async _ => {
          await ProcessQueueWithTimerControl().ConfigureAwait(false);
        }, null, TimeSpan.FromSeconds(Cx.IndexTimerIntervalSec), TimeSpan.FromSeconds(Cx.IndexTimerIntervalSec));

        _logger.LogDebug($"▶️ Queue processing timer started ({Cx.IndexTimerIntervalSec} second interval)");
      }
    }

    public void StopTimer() {
      lock (_processTimerLock) {
        if (_processTimer == null) {
          _logger.LogDebug("⚠️ Timer already stopped, skipping stop");
          return;
        }

        _processTimer.Dispose();
        _processTimer = null;
        _logger.LogDebug("⏹️ Queue processing timer stopped");
      }
    }

    public void StartScheduleTimer() {
      lock (_scheduledCmdLock) {
        if (_scheduledCmdTimer != null) {
          _logger.LogDebug("⚠️ Schedule timer already running, skipping start");
          return;
        }
        _scheduledCmdTimer = new Timer(async _ => {
          await ExecNextScheduledCmd().ConfigureAwait(false);
        }, null, TimeSpan.FromSeconds(Cx.IndexSchIntervalSec), TimeSpan.FromSeconds(Cx.IndexSchIntervalSec));
        _logger.LogDebug($"▶️ Schedule timer started");
      }
    }

    public void StopScheduleTimer() {
      lock (_scheduledCmdLock) {
        if (_scheduledCmdTimer == null) {
          _logger.LogDebug("⚠️ Save timer already stopped, skipping stop");
          return;
        }
        _scheduledCmdTimer.Dispose();
        _scheduledCmdTimer = null;
        _logger.LogDebug("⏹️ Save timer stopped");
      }
    }

    private async Task ExecNextScheduledCmd() {
      if (_isDisposed) return;            
      StopScheduleTimer();                   // Stop the timer before processing
      try {
        _logger.LogDebug("⏸️ Save timer paused - starting save all indexes");
        var cmd = PopNextScheduledCmd();
        if (cmd != null) {          
          var project = cmd.Project;
          if (project != null && project.ProjectIndex != null) {
            if (cmd.OpType == IndexOpType.WriteIndex) {
              await project.ProjectIndex.WriteIndexAsync().ConfigureAwait(false);
              _logger.LogDebug($"✅ {cmd.Id} {cmd.OpType} {cmd.Project.Name}");
            }            
          } else { 
            _logger.LogDebug("⚠️ No project or project index found for scheduled command");
          }
        }
        
      } catch (Exception ex) {
        _logger.LogError(ex, "❌ Error ExecNextScheduledCmd: {Message}", ex.Message);
        
      } finally {
        // Restart the timer after processing
        if (!_isDisposed && (_schedule.Keys.Count > 0) ) {
          _logger.LogDebug("▶️ Restarting ScheduledCmd timer");
          StartScheduleTimer();
        }
      } // End of finally block
    }

    private long _scheduledCmdNo = 1;
    private ConcurrentDictionary<long, IndexScheduledCmd> _schedule = new();
    public void ScheduleWriteIndex(IndexProjectItem project) {
      var op = new IndexScheduledCmd(_scheduledCmdNo, IndexOpType.WriteIndex, project);
      Interlocked.Increment(ref _scheduledCmdNo);
      _schedule[op.Id] = op;
      StartScheduleTimer();
    }

    public IndexScheduledCmd? PopNextScheduledCmd() {
      if (_schedule.Keys.Count > 0) {
        var firstKey = _schedule.Keys.OrderBy(x => x).First();
        _schedule.TryRemove(firstKey, out var op);
        return op;
      }
      return null;
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
          var shouldRestart = false;
          foreach (var project in _projectIndexModels) {
            if (project != null && project.ProjectIndex != null
              && project.ProjectIndex.ChangeQueue != null
              && project.ProjectIndex.ChangeQueue.Count > 0) {
              shouldRestart = true; // If any project has changes, restart the timer
            }
          }
          if (shouldRestart && _isEnabled && !wasError) {
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

          var projectIndex = project.ProjectIndex;                    // work out the project index object.
          if (projectIndex == null) {
            throw new InvalidOperationException($"Project index not found for project: {project.Name}");
          }
          
          List<string> projectFiles = new();                                  // reset list of files to index by project 
          var fullPath = Path.GetFullPath( project.Path);
          var files = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories)
             .Where(file => _securityService.IsFileAllowed(file))             // get the list of files to index
             .ToHashSet(StringComparer.OrdinalIgnoreCase);          

          projectFiles.AddRange(files);                              // add to the list of files to index          

          try {  
            // Phase 2: Cleanup - Remove files that no longer exist
            var indexedFiles = projectIndex.GetAllFileItems();
            var filesToRemove = indexedFiles.Where(f => !files.Contains(f.FilePathName)).ToList();
            if (filesToRemove.Count > 0) {
              projectIndex.RemoveFileAndAllRelated(filesToRemove);
            }          

            foreach (var filePath in projectFiles) {
              try { 
                projectIndex = await ProcessFileAsync(projectIndex, filePath, IsResync).ConfigureAwait(false);
                _logger.LogDebug($"Indexed: {filePath}");
              } catch (Exception ex) {
                _logger.LogError(ex, $"Error processing file {filePath} in project {project.Name}: {ex.Message}");
              }
            }

          } catch (Exception ex) {
            _logger.LogError(ex, $"Error processing files for project {project.Name}: {ex.Message}");
          } finally {            
            ScheduleWriteIndex(project);
          }
          
        }
        var status = await GetIndexStatus().ConfigureAwait(false);        

        return OperationResult.CreateSuccess("GetIndex", "Index rebuilt successfully", status);
      } catch (Exception ex) {
        return OperationResult.CreateFailure("GetIndex", $"Index rebuild error: {ex.Message}", ex);
      }
    }

    private List<IndexClassItem> indexClassItemsByFile = new();
    private List<IndexMethodItem> indexMethodItemsByClass = new();
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

      indexClassItemsByFile = aProjectIndexModel.GetAllClassItems(indexFileItem.Id);

      // Parse the C# file to extract classes, methods, properties
      var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);
      var lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

      if (string.IsNullOrEmpty(fileContent)) {
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
          startLine = FindClassStartCutLine(lines, startLine);

          IndexClassItem indexClassItem = new IndexClassItem() {
            FileItemId = indexFileItem.Id,
            FileName = filePath,
            Namespace = namespaceName,
            Name = className,
            LineStart = startLine,
            LineEnd = endLine
          };
          indexClassItem = aProjectIndexModel.AddUpdateClassItem(indexClassItem);       // update to table
          indexClassItemsByFile.Subtract(indexClassItem);                               // remove from in-memory list of items to delete

          var methods = classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Select(m => new IndexMethodItem() {
              Name = m.Identifier.Text,
              ClassId = indexClassItem.Id,
              ReturnType = m.ReturnType.ToString(),
              Parameters = JsonSerializer.Serialize(m.ParameterList.Parameters.Select(p => new { Type = p.Type.ToString(), Name = p.Identifier.Text }).ToList()),
              LineStart = m.GetLocation().GetLineSpan().StartLinePosition.Line,
              LineEnd = m.GetLocation().GetLineSpan().EndLinePosition.Line 
            }).ToList();

          indexMethodItemsByClass = aProjectIndexModel.GetAllMethodItemsByClass(indexClassItem.Id);

          foreach (var method in methods) {
            aProjectIndexModel.AddUpdateMethodItem(method);
            indexMethodItemsByClass.Subtract(method);                      // remove from in-memory list of items to delete
          }

          foreach (var remMethod in indexMethodItemsByClass) {             // remove any methods not found in this pass for class.
            aProjectIndexModel.DeleteMethodItem(remMethod);
          }
          /*   Removing until needed.       
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
          }  */

        } // End of classes in namespace

        foreach (var remClass in indexClassItemsByFile) {  // remove any classes not found in this pass for file.
          aProjectIndexModel.DeleteClassItem(remClass);
        }
      }  // End of namespace declarations

      return aProjectIndexModel;
    }

    private int FindClassStartCutLine(string[]? lines, int reportedStartLine) {
      // sanity check is a bit more. reported class is inside start line but attributes or comments pushes that back. 
      // so we backtrack to find the line to cut from to include those.
      if (lines == null || lines.Length == 0) return reportedStartLine;
      bool ClassFound = false;
      int classLine = -1;
      int startLine = reportedStartLine;

      if (startLine > 0) { 
        var line = lines[startLine].Trim();
        if (line.StartsWith("class ") || line.Contains(" class ")) {
          ClassFound = true;
          classLine = startLine;
        } else { 
          if (startLine +1 < lines.Length) {
            line = lines[startLine+1].Trim();
            if (line.StartsWith("class ") || line.Contains(" class ")) {
              ClassFound = true;
              classLine = startLine + 1;
            } else if (startLine -1>=0) {
              line = lines[startLine-1].Trim();
              if (line.StartsWith("class ") || line.Contains(" class ")) {
                ClassFound = true;
                classLine = startLine - 1;
              }
            }
          }
          if (!ClassFound && startLine + 2 < lines.Length) {
            line = lines[startLine + 2].Trim();
            if (line.StartsWith("class ") || line.Contains(" class ")) {
              ClassFound = true;
              classLine = startLine + 2;
            } else if (startLine - 2 >= 0) {
              line = lines[startLine - 2].Trim();
              if (line.StartsWith("class ") || line.Contains(" class ")) {
                ClassFound = true;
                classLine = startLine - 2;
              }
            }
          }
        }
      }

      if (ClassFound) {
        startLine = classLine;
        while (startLine > 0 && startLine - 1 > 0) {
          var line = lines[startLine - 1].Trim();
          if (line.Contains("}") || line.Contains("{") || line == "") {  // Stop at block boundaries or empty lines
            break; // Found the class declaration line
          }
          startLine--;
        }
      }

      return startLine;
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
      ScheduleWriteIndex(project);      
     
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
          _logger.LogDebug($"Re-index skipped: {Path.GetFileName(filePath)} detected no change");
          return; // No meaningful change
        }

        await ProcessFileAsync(project.ProjectIndex, filePath, false);

        _logger.LogDebug($"Re-indexed: {Path.GetFileName(filePath)}");

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
