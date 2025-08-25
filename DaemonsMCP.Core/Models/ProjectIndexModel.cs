using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace DaemonsMCP.Core.Models {

  /// <summary>
  /// ProjectIndexModel represents a single projects index.  
  /// It needs to hold the PackedTableSet of tables for each project.  
  /// This will be the item in the _projectIndexs dictionary.
  /// </summary>
  public class ProjectIndexModel : IDisposable {
    private volatile bool _isDisposed = false;
    private readonly ILogger<ProjectIndexModel> _logger;
    private readonly ProjectIndexWatchService? _watchService = null;  // reference to child file watching component.
    private readonly IValidationService _validationService;
    private readonly ProjectModel _project;

    public string ProjectName { get; set; } = string.Empty;  // key for the dictionary
    public string IndexFolderPath { get; set; } = string.Empty; // full path to the index folder for project.
    public string IndexFilePath { get; set; } = string.Empty;  // full path to the index file for project. 
    public string ProjectPath { get; set; } = string.Empty; // full path to the project directory root.
    public PackedTableSet IndexTables { get; private set; } = new PackedTableSet();
    public DateTime LastModified { get; set; } = DateTime.Now;    

    public IndexService? IndexService { get; set; } = null;  // public reference to its owner
    public ConcurrentQueue<FileChangeItem>? ChangeQueue => _watchService?.ChangeQueue;

    public TableModel Files { get; set; }
    public TableModel Classes { get; set; }
    public TableModel Methods { get; set; }
    public TableModel Properties { get; set; }
    public TableModel Events { get; set; }

    public ProjectIndexModel(ILoggerFactory loggerFactory, ProjectModel project, IValidationService validationService ) 
    {
      _logger = loggerFactory.CreateLogger<ProjectIndexModel>() ?? throw new ArgumentNullException(nameof(loggerFactory));
      _project = project ?? throw new ArgumentNullException(nameof(project));
      _validationService = validationService;
      IndexFolderPath = project.IndexPath;
      ProjectName = project.Name;
      ProjectPath = project.Path;

      IndexFilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(IndexFolderPath, $"{project.Name}_index.pktbs"));
      if (!Directory.Exists(IndexFolderPath)) {
        Directory.CreateDirectory(IndexFolderPath);
      }
      IndexTables.LoadFromFile(IndexFilePath);

      Files = IndexTables[Cx.FileTbl];
      if (Files == null) {
        Files = IndexTables.MakeFilesTable();          
      } 

      Classes = IndexTables[Cx.ClassesTbl];
      if (Classes == null) {
        Classes = IndexTables.MakeClassesTable();          
      }

      Methods = IndexTables[Cx.MethodsTbl];
      if (Methods == null) {
        Methods = IndexTables.MakeMethodsTable();          
      }

      Properties = IndexTables[Cx.PropertiesTbl];
      if (Properties == null) {
        Properties = IndexTables.MakePropertiesTable();          
      }

      Events = IndexTables[Cx.EventsTbl];
      if (Events == null) {
        Events = IndexTables.MakeEventsTable();          
      }
      _watchService = new ProjectIndexWatchService(loggerFactory, this);
    }

    public void Dispose() {
      if (_isDisposed) return;
      _isDisposed = true;
      _watchService?.Dispose();
      _logger.LogDebug($"ProjectIndexModel Dispose");
    }

    public void SetWatchServiceEnabled(bool enabled) { 
      if (_watchService != null) {
        if (enabled) {
          _watchService.StartWatching();
        } else {
          _watchService.StopWatching();
        }
      }
    }

    

    private void ReCreateTables() {
      IndexTables.LoadFromFile(IndexFilePath);
      Files = IndexTables.MakeFilesTable();
      Classes = IndexTables.MakeClassesTable();
      Methods = IndexTables.MakeMethodsTable();
      Properties = IndexTables.MakePropertiesTable();
      Events = IndexTables.MakeEventsTable();
    }

    public void WriteIndex() {
      if (IndexTables != null && IndexTables.TableCount > 0) {
        IndexTables.SaveToFile(IndexFilePath);
        LastModified = DateTime.Now;
      } else {
        throw new InvalidOperationException("No tables to write to index file.");
      }
    }


    #region IndexFileItem methods 
    public IndexFileItem? GetFileItemByPathName(string filePathName) {
      if (Files.Rows.IsEmpty) {
        return null; // No files in the index
      }        
      if (Files.FindFirst(Cx.FilePathNameCol, filePathName)) {
        var row = Files.Current;
        return this.GetFileItemById( row.Id);
      } else {
        return null; // File not found
      }
    }
    public int GetFileCount() {
      return Files?.Rows?.Count ?? 0;
    }
    public List<IndexFileItem> GetAllFileItems() {
      var fileItems = new List<IndexFileItem>(Files?.Rows?.Count ?? 0);
      if ( Files != null) {              
        foreach (var row in Files.Rows) {
          var fileItem = GetFileItemById(row.Key);
          if (fileItem != null) {                
            fileItems.Add(fileItem);
          }
        }
      }
      return fileItems;
    }
    public IndexFileItem? GetFileItemById(int id) {
      if (Files.Rows.TryGetValue(id, out RowModel? row)) {
        return new IndexFileItem() {
          Id = row[Cx.FileIdCol].Value.AsInt32(),
          FilePathName = row[Cx.FilePathNameCol].ValueString,
          Size = row[Cx.FileSizeCol].Value.AsInt64(),
          Modified = row[Cx.FileModifiedCol].Value.AsDateTime()
        };
      } else { return null; }
    }
    public void InsertFileItem(IndexFileItem item) {
      var row = Files.AddRow();
      item.Id = row.Id; // assign the new Id back to the item.
      row[Cx.FileIdCol].Value = item.Id;
      row[Cx.FilePathNameCol].Value = item.FilePathName;
      row[Cx.FileSizeCol].Value = item.Size;
      row[Cx.FileModifiedCol].Value = item.Modified;
      Files.Post();
    }
    public void UpdateFileItem(IndexFileItem item) {
      if (this.Files == null) {
        throw new InvalidOperationException("Files table is not initialized.");
      }
      var hasrow = this.Files.Rows.ContainsKey(item.Id);
      if (!hasrow) {
        throw new ArgumentException($"File with Id {item.Id} does not exist in the index.");
      }
      this.Files.FindFirst(Cx.FileIdCol, item.Id);
      Files.Edit();
      var row = Files.Current;              
      row[Cx.FilePathNameCol].Value = item.FilePathName;
      row[Cx.FileSizeCol].Value = item.Size;
      row[Cx.FileModifiedCol].Value = item.Modified;
      Files.Post();
    }
    public void DeleteFileItem(IndexFileItem item) {
      if (Files.FindFirst(Cx.FileIdCol, item.Id)) {
        ClearClassIndex(item); // Clear classes associated with this file
        Files.DeleteCurrentRow();
      }
    }

    public void RemoveFileAndAllRelated(List<IndexFileItem> items) {
      if (items == null) {
        throw new ArgumentNullException(nameof(items), "Items to remove cannot be null");
      }
      foreach (var item in items) {
        if (item != null) {
          DeleteFileItem(item);
        }
      }      
    }

    #endregion

    #region IndexClassItem methods 
    public int GetClassCount() {
      return Classes?.Rows?.Count ?? 0;
    }

    public IndexClassItem? GetClassByName( string classNamespace, string className) {
      if (Classes.Rows.IsEmpty || string.IsNullOrWhiteSpace(classNamespace) || string.IsNullOrWhiteSpace(className)) {
        return null; // No classes in the index
      }        
      var found = Classes.Rows.Where(c => c.Value[Cx.ClassesNamespaceCol].ValueString == classNamespace &&
      c.Value[Cx.ClassesNameCol].ValueString == className).Select(c => c.Value);
      if (found.Any()) {
         var row = found.First();
         return this.GetClassById(row.Id);
      }      
      return null; // Class not found      
    }

  

    public IndexClassItem? GetClassById(int id) {
      if (Classes.Rows.TryGetValue(id, out RowModel? row)) {
        return new IndexClassItem() {
          Id = row[Cx.ClassesIdCol].Value.AsInt32(),
          FileItemId = row[Cx.ClassesFileIdCol].Value.AsInt32(),
          Name = row[Cx.ClassesNameCol].ValueString,
          Namespace = row[Cx.ClassesNamespaceCol].ValueString,
          FileName = row[Cx.ClassesFileNameCol].ValueString,
          LineStart = (int)row[Cx.ClassesLineStartCol].Value,
          LineEnd = (int)row[Cx.ClassesLineEndCol].Value
        };
      } else { return null; }
    }
    public IndexClassItem InsertClassItem(IndexClassItem item) {
      var Row = Classes.AddRow();
      item.Id = Row.Id; // assign the new Id back to the item.
      Row[Cx.ClassesIdCol].Value = item.Id;
      Row[Cx.ClassesFileIdCol].Value = item.FileItemId;
      Row[Cx.ClassesNameCol].Value = item.Name;
      Row[Cx.ClassesNamespaceCol].Value = item.Namespace;
      Row[Cx.ClassesFileNameCol].Value = item.FileName;
      Row[Cx.ClassesLineStartCol].Value = item.LineStart;
      Row[Cx.ClassesLineEndCol].Value = item.LineEnd;
      Classes.Post();
      return item;
  }
    public void UpdateClassItem(IndexClassItem item) {
      var hasrow = this.Classes.Rows.ContainsKey(item.Id);
      if (!hasrow) {
        throw new ArgumentException($"Class with Id {item.Id} does not exist in the index.");
      }
      this.Classes.FindFirst(Cx.ClassesIdCol, item.Id);
      Classes.Edit();
      var row = Classes.Current;
      row[Cx.ClassesIdCol].Value = item.Id;
      row[Cx.ClassesFileIdCol].Value = item.FileItemId;
      row[Cx.ClassesNameCol].ValueString = item.Name;
      row[Cx.ClassesNamespaceCol].ValueString = item.Namespace;
      row[Cx.ClassesFileNameCol].Value = item.FileName;
      row[Cx.ClassesLineStartCol].Value = item.LineStart;
      row[Cx.ClassesLineEndCol].Value = item.LineEnd;
      Classes.Post();
    }
    public void DeleteClassItem(IndexClassItem item) {
      if (Classes.FindFirst(Cx.ClassesIdCol, item.Id)) {
        ClearMethodIndex(item.Id); // Clear methods associated with this class
        ClearPropertyIndex(item.Id); // Clear properties associated with this class
        ClearEventIndex(item.Id); // Clear events associated with this class
        Classes.DeleteCurrentRow();
      }
    }
    public void ClearClassIndex(IndexFileItem fileItem) {
      if (fileItem == null) {
        throw new ArgumentNullException(nameof(fileItem), "File item cannot be null.");
      }
      if (Classes == null) {
        throw new InvalidOperationException("Classes table is not initialized.");
      }
      var idsToDelete = Classes.Rows
        .Where(r => r.Value[Cx.ClassesFileIdCol].Value.AsInt32() == fileItem.Id)
        .Select(r => r.Key)
        .ToList();     
      
      foreach (var id in idsToDelete) {
        if (Classes.FindFirst(Cx.ClassesIdCol, id)) {
          var classId = Classes.Current.Id;
          ClearMethodIndex(classId); // Clear methods associated with this class
          ClearPropertyIndex(classId); // Clear properties associated with this class
          ClearEventIndex(classId); // Clear events associated with this class
          Classes.DeleteCurrentRow();
        }
      }
    }

    #endregion

    #region IndexMethodItem methods
    public int GetMethodCount() {
      return Methods?.Rows?.Count ?? 0;
    }

    public IndexMethodItem? GetMethodById(int id) {
      if (Methods.Rows.TryGetValue(id, out RowModel? row)) {
        return new IndexMethodItem() {
          Id = row[Cx.MethodsIdCol].Value.AsInt32(),
          ClassId = row[Cx.MethodsClassIdCol].Value.AsInt32(),
          Name = row[Cx.MethodsNameCol].ValueString,
          Parameters = row[Cx.MethodsParametersCol].ValueString,
          ReturnType = row[Cx.MethodsReturnTypeCol].ValueString,
          LineStart = (int)row[Cx.MethodsLineStartCol].Value,
          LineEnd = (int)row[Cx.MethodsLineEndCol].Value
        };
      } else { return null; }
    }

    public IndexMethodItem? GetMethodByName(string namespaceName, string className, string methodName) {
      if (Classes.Rows.IsEmpty || Methods.Rows.IsEmpty) {
        return null;
      }
      int classId = 0;
      var found = Classes.Rows.Where(c => c.Value[Cx.ClassesNamespaceCol].ValueString == namespaceName &&
      c.Value[Cx.ClassesNameCol].ValueString == className).Select(c => c.Value);
      if (found.Any()) {
        var row = found.First();
        classId = row.Id;
        var methodFound = Methods.Rows.Where(r => (int)r.Value[Cx.MethodsClassIdCol].Value == classId
          && r.Value[Cx.MethodsNameCol].ValueString == methodName);
        if (methodFound.Any()) {
          var methodRow = methodFound.First().Value;
          return GetMethodById(methodRow.Id);
        }
      }
      return null; // Method not found
    }

    public IndexMethodItem InsertMethodItem(IndexMethodItem item) {
      var Row = Methods.AddRow();
      item.Id = Row.Id; // assign the new Id back to the item.
      Row[Cx.MethodsIdCol].Value = item.Id;
      Row[Cx.MethodsClassIdCol].Value = item.ClassId;
      Row[Cx.MethodsNameCol].Value = item.Name;
      Row[Cx.MethodsParametersCol].ValueString = item.Parameters;
      Row[Cx.MethodsReturnTypeCol].ValueString = item.ReturnType;
      Row[Cx.MethodsLineStartCol].Value = item.LineStart;
      Row[Cx.MethodsLineEndCol].Value = item.LineEnd;
      Methods.Post();
      return item;
    }

    public void UpdateMethodItem(IndexMethodItem item) {
      var hasrow = this.Methods.Rows.ContainsKey(item.Id);
      if (!hasrow) {
        throw new ArgumentException($"Method with Id {item.Id} does not exist in the index.");
      }
      this.Methods.FindFirst(Cx.MethodsIdCol, item.Id);
      Methods.Edit();
      var row = Methods.Current;
      row[Cx.MethodsIdCol].Value = item.Id;
      row[Cx.MethodsClassIdCol].Value = item.ClassId;
      row[Cx.MethodsNameCol].ValueString = item.Name;
      row[Cx.MethodsReturnTypeCol].ValueString = item.ReturnType;
      row[Cx.MethodsParametersCol].Value = item.Parameters;
      row[Cx.MethodsLineStartCol].Value = item.LineStart;
      row[Cx.MethodsLineEndCol].Value = item.LineEnd;
      Methods.Post();
    }

    public void DeleteMethodItem(IndexMethodItem item) {
      if (Methods.FindFirst(Cx.MethodsIdCol, item.Id)) {
        Methods.DeleteCurrentRow();
      }
    }

    public void ClearMethodIndex(int classId) {
      if (Methods == null) {
        throw new InvalidOperationException("Methods table is not initialized.");
      }

      var idsToDelete = Methods.Rows
        .Where(r => r.Value[Cx.MethodsClassIdCol].Value.AsInt32() == classId)
        .Select(r => r.Key)
        .ToList();

      foreach (var id in idsToDelete) {
        if (Methods.FindFirst(Cx.MethodsIdCol, id)) {
          Methods.DeleteCurrentRow();
        }
      }

    }

    #endregion

    #region IndexPropertyItem methods
    public IndexPropertyItem? GetPropertyById(int id) {
      if (Properties.Rows.TryGetValue(id, out RowModel? row)) {
        return new IndexPropertyItem() {
          Id = row.Id,
          ClassId = (int)row[Cx.PropertiesClassIdCol].Value,
          Name = row[Cx.PropertiesNameCol].ValueString,
          Type = row[Cx.PropertiesTypeCol].ValueString,
          LineStart = (int)row[Cx.PropertiesLineStartCol].Value,
          LineEnd = (int)row[Cx.PropertiesLineEndCol].Value,
          HasGetter = (bool)row[Cx.PropertiesHasGetterCol].Value,
          HasSetter = (bool)row[Cx.PropertiesHasSetterCol].Value
        };
      } else { return null; }
    }

    public IndexPropertyItem InsertPropertyItem(IndexPropertyItem item) {
      var Row = Properties.AddRow();
      item.Id = Row.Id; // assign the new Id back to the item.
      Row[Cx.PropertiesIdCol].Value = item.Id;
      Row[Cx.PropertiesClassIdCol].Value = item.ClassId;
      Row[Cx.PropertiesNameCol].ValueString = item.Name;
      Row[Cx.PropertiesTypeCol].ValueString = item.Type;
      Row[Cx.PropertiesLineStartCol].Value = item.LineStart;
      Row[Cx.PropertiesLineEndCol].Value = item.LineEnd;
      Row[Cx.PropertiesHasGetterCol].Value = item.HasGetter;
      Row[Cx.PropertiesHasSetterCol].Value = item.HasSetter;
      Properties.Post();
      return item;
    }

    public void UpdatePropertyItem(IndexPropertyItem item) {
      var hasrow = this.Properties.Rows.ContainsKey(item.Id);
      if (!hasrow) {
        throw new ArgumentException($"Property with Id {item.Id} does not exist in the index.");
      }
      this.Properties.FindFirst(Cx.PropertiesIdCol, item.Id);
      Properties.Edit();
      var row = Properties.Current;
      row[Cx.PropertiesIdCol].Value = item.Id;
      row[Cx.PropertiesClassIdCol].Value = item.ClassId;
      row[Cx.PropertiesNameCol].ValueString = item.Name;
      row[Cx.PropertiesTypeCol].ValueString = item.Type;
      row[Cx.PropertiesLineStartCol].Value = item.LineStart;
      row[Cx.PropertiesLineEndCol].Value = item.LineEnd;
      row[Cx.PropertiesHasGetterCol].Value = item.HasGetter;
      row[Cx.PropertiesHasSetterCol].Value = item.HasSetter;
      Properties.Post();
    }

    public void DeletePropertyItem(IndexPropertyItem item) {
      if (Properties.FindFirst(Cx.PropertiesIdCol, item.Id)) {
        Properties.DeleteCurrentRow();
      }
    }

    public void ClearPropertyIndex(int classId) {
      if (Properties == null) {
        throw new InvalidOperationException("Properties table is not initialized.");
      }
      var idsToDelete = Properties.Rows
        .Where(r => r.Value[Cx.PropertiesClassIdCol].Value.AsInt32() == classId)
        .Select(r => r.Key)
        .ToList();
     
      foreach (var id in idsToDelete) {
        if (Properties.FindFirst(Cx.PropertiesIdCol, id)) {
          Properties.DeleteCurrentRow();
        }
      }
    }

    #endregion

    #region IndexEventItem methods
    public IndexEventItem? GetEventById(int id) {
      if (Events.Rows.TryGetValue(id, out RowModel? row)) {
        return new IndexEventItem() {
          Id = row.Id,
          ClassId = (int)row[Cx.EventsClassIdCol].Value,
          Name = row[Cx.EventsNameCol].ValueString,
          Type = row[Cx.EventsTypeCol].ValueString,
          LineStart = (int)row[Cx.EventsLineStartCol].Value,
          LineEnd = (int)row[Cx.EventsLineEndCol].Value
        };
      } else { return null; }
    }

    public IndexEventItem InsertEventItem(IndexEventItem item) {
      var Row = Events.AddRow();
      item.Id = Row.Id; // assign the new Id back to the item.
      Row[Cx.EventsIdCol].Value = item.Id;
      Row[Cx.EventsClassIdCol].Value = item.ClassId;
      Row[Cx.EventsNameCol].ValueString = item.Name;
      Row[Cx.EventsTypeCol].ValueString = item.Type;
      Row[Cx.EventsLineStartCol].Value = item.LineStart;
      Row[Cx.EventsLineEndCol].Value = item.LineEnd;        
      Events.Post();
      return item;
    }

    public void UpdateEventItem(IndexEventItem item) {
      var hasrow = this.Events.Rows.ContainsKey(item.Id);
      if (!hasrow) {
        throw new ArgumentException($"Event with Id {item.Id} does not exist in the index.");
      }
      this.Events.FindFirst(Cx.EventsIdCol, item.Id);
      Events.Edit();
      var row = Events.Current;
      row[Cx.EventsIdCol].Value = item.Id;
      row[Cx.EventsClassIdCol].Value = item.ClassId;
      row[Cx.EventsNameCol].ValueString = item.Name;
      row[Cx.EventsTypeCol].ValueString = item.Type;
      row[Cx.EventsLineStartCol].Value = item.LineStart;
      row[Cx.EventsLineEndCol].Value = item.LineEnd;        
      Events.Post();
    }

    public void DeleteEventItem(IndexEventItem item) {
      if (Events.FindFirst(Cx.EventsIdCol, item.Id)) {
        Events.DeleteCurrentRow();
      }
    }

    public void ClearEventIndex(int classId) {
      if (Events == null) {
        throw new InvalidOperationException("Events table is not initialized.");
      }
      var idsToDelete = Events.Rows
        .Where(r => r.Value[Cx.EventsClassIdCol].Value.AsInt32() == classId)
        .Select(r => r.Key)
        .ToList();
            
      foreach (var id in idsToDelete) {
        if (Events.FindFirst(Cx.EventsIdCol, id)) {
          Events.DeleteCurrentRow();
        }
      }
    }

    #endregion

    #region ClassContent methods
    public async Task<List<ClassListing>> GetClassListingsAsync(int PageNo = 1, int maxResults = 100, string? namespaceFilter = null, string? classNameFilter = null) {
      var results = new List<ClassListing>();
      if (Classes.Rows.IsEmpty) {
        return results; // Return empty list if no classes in index
      }
      try {
        bool isNamespaceFilter = !string.IsNullOrWhiteSpace(namespaceFilter);
        bool isClassNameFilter = !string.IsNullOrWhiteSpace(classNameFilter);

        int skipTo = (PageNo - 1) * maxResults;
        int found = 0;
        int taken = 0;
        int rowNum = 1;
        bool isNamespaceMatch = false;
        bool isClassNameMatch = false;
        Classes.MoveFirst();
        while (Classes.Current != null) {
          isNamespaceMatch = false;
          isClassNameMatch = false;

          var classItem = GetClassById(Classes.Current.Id);
          if (classItem != null) {
            var fileItem = GetFileItemById(classItem.FileItemId);
            if (fileItem != null) {

              isNamespaceMatch = isNamespaceFilter && Classes.Current[Cx.ClassesNamespaceCol].ValueString.Contains(namespaceFilter, StringComparison.OrdinalIgnoreCase);
              isClassNameMatch = isClassNameFilter && Classes.Current[Cx.ClassesNameCol].ValueString.Contains(classNameFilter, StringComparison.OrdinalIgnoreCase);

              if (isNamespaceMatch || isClassNameMatch || (!isNamespaceFilter && !isClassNameFilter)) {
                found++;
                if (found > skipTo) {
                  var relativePath = fileItem.FilePathName.Replace(ProjectPath, "").TrimStart(System.IO.Path.DirectorySeparatorChar);
                  results.Add(new ClassListing() {
                    ClassId = classItem.Id,
                    FileId = fileItem.Id,
                    ClassName = classItem.Name,
                    Namespace = classItem.Namespace,
                    FileNamePath = relativePath,
                    LineStart = classItem.LineStart,
                    LineEnd = classItem.LineEnd,
                    ProjectName = this.ProjectName,
                    Modified = fileItem.Modified
                  });
                  taken++;
                }
              }
            }
          }
          if (taken >= maxResults) {
            break; // Stop if we've reached the maximum number of results
          }

          Classes.MoveNext();
          rowNum++;
        }


      } catch (Exception ex) {
        _logger.LogError(ex, "Error during class listing {namespaceFilter} {classNameFilter}", namespaceFilter, classNameFilter);
      }
      return results;
    }

    public async Task<ClassContent> GetClassContentById(int ClassId) { 
      try { 

        var classItem = GetClassById(ClassId);
        if (classItem != null) {
          
          var fullPath = classItem.FileName;
          if (File.Exists(fullPath)) {
            var allLines = await File.ReadAllLinesAsync(fullPath).ConfigureAwait(false);
            var lines = new List<string>(allLines.Length);
            int currentLine = 1;
            int lineEndAt = classItem.LineEnd + 1;
            int stopAt = lineEndAt + 1;
            foreach (string line in allLines) {
              if (currentLine >= stopAt) break;
              if (currentLine >= classItem.LineStart && currentLine <= lineEndAt) {
                lines.Add(line);
              }
              currentLine++;
            }
            if (lines.Any<string>()) {              
              var content = string.Join(Environment.NewLine, lines);
              var relativePath = classItem.FileName.Replace(ProjectPath, "").TrimStart(System.IO.Path.DirectorySeparatorChar);
              return new ClassContent() {
                ClassId = classItem.Id,
                ClassName = classItem.Name,
                FileNamePath = relativePath,
                Namespace = classItem.Namespace,
                Content = content
              };
            } else {
              _logger.LogWarning("File {FilePath} has only {LineCount} lines, cannot get lines {LineStart}-{LineEnd}", fullPath, allLines.Length, classItem.LineStart, classItem.LineEnd);
            }
          } else {
            _logger.LogWarning("File {FilePath} does not exist for ClassId {ClassId}", fullPath, ClassId);
          }
          
        } else {
          _logger.LogWarning("ClassId {ClassId} not found in index", ClassId);
        }
       

      } catch (Exception ex) { 
        _logger.LogError(ex, "Error getting class content for ClassId {ClassId}", ClassId);
      }

      return new ClassContent(); // return empty if not found.
    }

    public async Task<OperationResult> AddUpdateClassAsync(ClassContent classContent) { 
      try { 

        if (classContent == null 
          || string.IsNullOrWhiteSpace(classContent.Namespace) 
          || string.IsNullOrEmpty(classContent.ClassName)) { 
          throw new ArgumentException ("Bad input. Namespace, ClassName are required." );
        }
        var filePathName = classContent.FileNamePath;
        var content = classContent.Content;
        bool existingClassFound = false;        
        IndexClassItem? existingClass = null;

        int ClassId = classContent.ClassId;
        if (ClassId > 0) {                                 // >0 means update existing class.
          existingClassFound = true;
          existingClass = GetClassById(ClassId);       // get existing class info
          if (existingClass == null) {
              existingClassFound = false;
          }                                                 // err didn't find it though.
        }

        _validationService.ValidatePath(filePathName);
        _validationService.ValidateContent(content);
        var context = _validationService.ValidateAndPrepare(ProjectName, filePathName, false);
        var fullPath = context.FullPath;
        _validationService.ValidatePrepToSave(ProjectPath, fullPath, content, true);

        // Create directory if needed
        var directory = System.IO.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          Directory.CreateDirectory(directory);          
        }

        if (File.Exists(fullPath)) {
          context.Project.CopyToBackup(fullPath);
        }
        bool isNewFile = false;
        if (existingClassFound ) {
          
          var oldFileItem = GetFileItemById(existingClass.FileItemId);
          if (oldFileItem != null) {
            ClearClassIndex(oldFileItem); // Remove old class and related items                              

            var allLines = await File.ReadAllLinesAsync(existingClass.FileName).ConfigureAwait(false);
            var lines = new List<string>(allLines.Length);
            int currentLine = 1;
            int lineEndAt = existingClass.LineEnd + 1;
            bool hasInserted = false;
            foreach (string line in allLines) {
            
              if (currentLine >= existingClass.LineStart && currentLine <= lineEndAt) {
                if (!hasInserted) {
                  lines.Add(content); // insert new content
                  hasInserted = true;
                }
              } else {
                lines.Add(line);
              }
              currentLine++;
            }
            if (!hasInserted) {
              int lastLine = lines.Count-1;
              if (lastLine < 0) lastLine = 0;
              lines.Insert(lastLine, content); // append new content if not inserted
            }
            if (! string.IsNullOrWhiteSpace( classContent.UsesClauseAdd)) { 
               lines.Insert(0, classContent.UsesClauseAdd); // add uses clause if provided 
            }
            content = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(fullPath, content).ConfigureAwait(false);  // write to new path

            if (string.Compare(oldFileItem.FilePathName, fullPath) != 0) {
              File.Delete(oldFileItem.FilePathName); // delete old file if path changed
            }

          } else { 
            isNewFile = true;
          }
        }

        if (isNewFile) { 
          StringBuilder sb = new StringBuilder();
          if ( !string.IsNullOrWhiteSpace( classContent.UsesClauseAdd)) { 
            sb.AppendLine(classContent.UsesClauseAdd);
          }
          sb.AppendLine(Environment.NewLine);
          sb.AppendLine($"namespace {classContent.Namespace} {{"+Environment.NewLine);
          sb.AppendLine(content);
          sb.AppendLine(Environment.NewLine+"}}");
          string newContent =  sb.ToString();
          await File.WriteAllTextAsync(fullPath, newContent).ConfigureAwait(false);

        }

        await this.IndexService.ProcessFileAsync(this, fullPath, true).ConfigureAwait(false);

        var newClassIndex = GetClassByName(classContent.Namespace, classContent.ClassName); // get the new class info
        var newClassContent = await GetClassContentById(newClassIndex.Id).ConfigureAwait(false);

        var opResult = OperationResult.CreateSuccess(Cx.AddUpdateClassCmd, $"{Cx.AddUpdateClassCmd} Success.", newClassContent);
        return opResult;

      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(
          Cx.AddUpdateClassCmd,
          $"Failed to update class: {ex.Message}",
          ex
        );
        return opResult;
      }
    }

    #endregion

    public async Task<List<MethodListing>> GetMethodListingsAsync(int PageNo = 1, int maxResults = 100, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter = null) {
      var results = new List<MethodListing>();
      if (Methods.Rows.IsEmpty) {
        return results; // Return empty list if no methods in index
      }
      try {
        bool isNamespaceFilter = !string.IsNullOrWhiteSpace(namespaceFilter);
        bool isMethodNameFilter = !string.IsNullOrWhiteSpace(methodNameFilter);
        bool isClassNameFilter = !string.IsNullOrWhiteSpace(classNameFilter);

        int skipTo = (PageNo - 1) * maxResults;
        int found = 0;
        int taken = 0;
        int rowNum = 1;
        bool isNamespaceMatch = false;
        bool isMethodNameMatch = false;
        bool isClassNameMatch = false;
        Methods.MoveFirst();
        while (Methods.Current != null) {
          isMethodNameMatch = false;
          isClassNameMatch = false;
          isNamespaceMatch = false;

          var classItem = GetClassById(Methods.Current[Cx.MethodsClassIdCol].Value.AsInt32());
          if (classItem != null) {
            isMethodNameMatch = isMethodNameFilter && Methods.Current[Cx.MethodsNameCol].ValueString.Contains(methodNameFilter, StringComparison.OrdinalIgnoreCase);
            isClassNameMatch = isClassNameFilter && classItem.Name.Contains(classNameFilter, StringComparison.OrdinalIgnoreCase);
            isNamespaceMatch = isNamespaceFilter && classItem.Namespace.Contains(namespaceFilter, StringComparison.OrdinalIgnoreCase);
            if (isMethodNameMatch || isClassNameMatch || isNamespaceMatch 
              || (!isMethodNameFilter && !isClassNameFilter && !isNamespaceMatch)) 
            {
              found++;
              if (found > skipTo) {
                var relativePath = classItem.FileName.Replace(ProjectPath, "").TrimStart(System.IO.Path.DirectorySeparatorChar);
                results.Add(new MethodListing() {
                  MethodId = Methods.Current.Id,
                  ClassId = classItem.Id,
                  ProjectName = this.ProjectName,
                  Namespace = classItem.Namespace,
                  ClassName = classItem.Name,
                  MethodName = Methods.Current[Cx.MethodsNameCol].ValueString,
                  Parameters = Methods.Current[Cx.MethodsParametersCol].ValueString,
                  ReturnType = Methods.Current[Cx.MethodsReturnTypeCol].ValueString,                    
                  LineStart = classItem.LineStart,
                  LineEnd = classItem.LineEnd,
                  FileName = relativePath
                });
                taken++;
              }
            }
            
          }
          if (taken >= maxResults) {
            break; // Stop if we've reached the maximum number of results
          }

          Methods.MoveNext();
          rowNum++;
        }
       
      } catch (Exception ex) {
        _logger.LogError(ex, "Error during method listing ");
      }
      return results;
    }

    public async Task<MethodContent> GetMethodContentById(int MethodId) {
      try {

        var methodItem = GetMethodById(MethodId);
        if (methodItem != null) {
          var classItem = GetClassById(methodItem.ClassId);
          if (classItem != null) {

            var fullPath = classItem.FileName;
            if (File.Exists(fullPath)) {

              var allLines = await File.ReadAllLinesAsync(fullPath).ConfigureAwait(false);
              var lines = new List<string>(allLines.Length);
              int currentLine = 1;
              int lineEndAt = methodItem.LineEnd + 1;
              int stopAt = lineEndAt + 1;
              foreach (string line in allLines) {
                if (currentLine >= stopAt) break;
                if (currentLine >= methodItem.LineStart && currentLine <= lineEndAt) {
                  lines.Add(line);
                }
                currentLine++;
              }
              if (lines.Any<string>()) {
                var content = string.Join(Environment.NewLine, lines);
                var relativePath = classItem.FileName.Replace(ProjectPath, "").TrimStart(System.IO.Path.DirectorySeparatorChar);
                return new MethodContent() {
                  MethodId = methodItem.Id,
                  ClassId = classItem.Id,
                  Namespace = classItem.Namespace,
                  ClassName = classItem.Name,
                  MethodName = methodItem.Name,
                  FileNamePath = relativePath,                  
                  Content = content
                };

              } else {
                _logger.LogWarning("File {FilePath} has only {LineCount} lines, cannot get lines {LineStart}-{LineEnd}", fullPath, allLines.Length, methodItem.LineStart, methodItem.LineEnd);
              }

            } else { 
                _logger.LogWarning("File {FilePath} does not exist for MethodId {MethodId}", fullPath, MethodId);
            }

          } else {
            _logger.LogWarning("ClassID {ClassId} not found in index",  methodItem.ClassId);
          }

        } else {
          _logger.LogWarning("MethodId {MethodId} not found in index", MethodId);
        }


      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting method content for MethodId {MethodId}", MethodId);
      }

      return new MethodContent(); // return empty if not found.
    }

    public async Task<OperationResult> AddUpdateMethodContent(MethodContent methodContent) {
      try {

        if (methodContent == null
          || string.IsNullOrWhiteSpace(methodContent.Namespace)
          || string.IsNullOrEmpty(methodContent.ClassName)
          || string.IsNullOrWhiteSpace(methodContent.MethodName)
          || string.IsNullOrWhiteSpace(methodContent.Content)) {          
          throw new ArgumentException("Bad input. Namespace, ClassName, MethodName, Content are required.");
        }

        _validationService.ValidatePath(methodContent.FileNamePath); // validations throw errors on offenders
        _validationService.ValidateContent(methodContent.Content);
        var context = _validationService.ValidateAndPrepare(ProjectName, methodContent.FileNamePath, false);
        var fullPath = context.FullPath;  // destination path from user validated rooted.
        _validationService.ValidatePrepToSave(ProjectPath, fullPath, methodContent.Content, true);
        
        var content = methodContent.Content;
        
        bool existingMethodFound = false;
        IndexMethodItem? existingMethod = null;
        int MethodId = methodContent.MethodId;
        if (MethodId > 0) {                                 // >0 means update existing method.
          existingMethodFound = true;
          existingMethod = GetMethodById(MethodId);       // get existing method info
          if (existingMethod == null) {
            existingMethodFound = false;
          } else { 
            
          }
        }

        bool existingClassFound = false;
        IndexClassItem? existingClass = null;
        int ClassId = methodContent.ClassId;
        if (ClassId > 0) {                                 // >0 means update existing class.
          existingClassFound = true;
          existingClass = GetClassById(ClassId);       // get existing class info
          if (existingClass == null) {
            existingClassFound = false;
          }                                                 // err didn't find it though.
        }
        
        // Create directory if needed
        var directory = System.IO.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          Directory.CreateDirectory(directory);
        }

        if (File.Exists(fullPath)) {
          context.Project.CopyToBackup(fullPath);
        }
        bool isNewFile = false;
        if (existingClassFound && existingMethodFound) {

          var oldFileItem = GetFileItemById(existingClass.FileItemId);
          if (oldFileItem != null) {
            ClearClassIndex(oldFileItem); // Remove old class and related index items                              

            var allLines = await File.ReadAllLinesAsync(existingClass.FileName).ConfigureAwait(false);

            var lines = new List<string>(allLines.Length);
            int currentLine = 1;
            int lineEndAt = existingMethod.LineEnd + 1;
            bool hasInserted = false;
            foreach (string line in allLines) {
              if (currentLine >= existingMethod.LineStart && currentLine <= lineEndAt) {
                if (!hasInserted) {
                  lines.Add(content); // insert new content
                  hasInserted = true;
                }
              } else {
                lines.Add(line);
              }
              currentLine++;
            }
            if (!hasInserted) {
              int lastLine = lines.Count - 2;
              if (lastLine < 0) lastLine = 0;
              lines.Insert(lastLine, content); // append new content if not inserted
            }
            if (!string.IsNullOrWhiteSpace(methodContent.UsesClauseAdd)) {
              lines.Insert(0, methodContent.UsesClauseAdd); // add uses clause if provided 
            }
            content = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(fullPath, content).ConfigureAwait(false);  // write to new path                                                                                    

          } else {
            isNewFile = true;
          }
        }

        if (existingClassFound && !existingMethodFound) { 
          // adding new method to existing class.
          var oldFileItem = GetFileItemById(existingClass.FileItemId);
          if (oldFileItem != null) {
            ClearClassIndex(oldFileItem); // Remove old class and related index items                              
    
            var allLines = await File.ReadAllLinesAsync(existingClass.FileName).ConfigureAwait(false);
    
            var lines = new List<string>(allLines.Length);
            int currentLine = 1;
            int lineEndAt = existingClass.LineEnd + 1;
            bool hasInserted = false;
            foreach (string line in allLines) {
              if (currentLine >= existingClass.LineStart && currentLine <= lineEndAt) {
                if (!hasInserted && line.Trim().EndsWith("}")) {
                  lines.Add(content); // insert new content before the last }
                  hasInserted = true;
                  }
              }
              lines.Add(line);
              currentLine++;
            }
            if (!hasInserted) {
              int lastLine = lines.Count - 1;
              if (lastLine < 0) lastLine = 0;
              lines.Insert(lastLine, content); // append new content if not inserted
            }
            if (!string.IsNullOrWhiteSpace(methodContent.UsesClauseAdd)) {
            lines.Insert(0, methodContent.UsesClauseAdd); // add uses clause if provided 
            }
            content = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(fullPath, content).ConfigureAwait(false);  // write to new path                                                                                    
    
          } else {
              isNewFile = true;
          }
        }

        if (isNewFile) {
          StringBuilder sb = new StringBuilder();
          if (!string.IsNullOrWhiteSpace(methodContent.UsesClauseAdd)) {
            sb.AppendLine(methodContent.UsesClauseAdd);
          }
          sb.AppendLine(Environment.NewLine);
          sb.AppendLine($"namespace {methodContent.Namespace} {{" + Environment.NewLine);
          sb.AppendLine($"  public class {methodContent.ClassName} {{"+Environment.NewLine);
          sb.AppendLine(content);
          sb.AppendLine(Environment.NewLine + "  }}");
          sb.AppendLine(Environment.NewLine + "}}");
          string newContent = sb.ToString();
          await File.WriteAllTextAsync(fullPath, newContent).ConfigureAwait(false);
        }

        await this.IndexService.ProcessFileAsync(this, fullPath, true).ConfigureAwait(false);

        var newMethodIndex = GetMethodByName(methodContent.Namespace, methodContent.ClassName, methodContent.MethodName); // get the new class info
        var newMethodContent = await GetMethodContentById(newMethodIndex.Id).ConfigureAwait(false);

        var opResult = OperationResult.CreateSuccess(Cx.AddUpdateClassCmd, $"{Cx.AddUpdateClassCmd} Success.", newMethodContent);
        return opResult;

      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(
          Cx.AddUpdateClassCmd,
          $"Failed to update class: {ex.Message}",
          ex
        );
        return opResult;
      }
    }

  }
}
