using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DaemonsMCP.Core.Extensions {
  public static class Cx {
    public const string AppName = "DaemonsMCP";
    public const string AppVersion = "0.1.0";
    public const string CONFIG_FILE_NAME = "daemonsmcp.json";
    public const bool IsDebug = true;
    public const int IndexTimerIntervalSec = 3; // seconds
    public const string DaemonsFolderName = ".daemons";
    public const string BackupFolderName = "backups";

    // Tools Commands    
    public const string ToolListMethodsCmd = "tools/list";
    public const string ToolCallMethodCmd = "tools/call";
    public const string ToolInitializeCmd = "initialize";
    public const string ToolInitializedNotificationCmd = "notifications/initialized";
    public const string ToolListResourcesCmd = "resources/list";
    public const string ToolPromptsListCmd = "prompts/list";    

    // Tools Names
    public const string ListProjectsCmd = "list-projects";
    public const string ListFoldersCmd = "list-project-directories";
    public const string ListFilesCmd = "list-project-files";

    public const string GetFileCmd = "get-project-file";
    public const string InsertFileCmd = "create-project-file";
    public const string UpdateFileCmd = "update-project-file";
    public const string DeleteFileCmd = "delete-project-file";

    public const string CreateFolderCmd = "create-project-directory";
    public const string DeleteFolderCmd = "delete-project-directory";
        
    public const string ResyncIndexCmd = "resync-index";
    public const string StatusIndexCmd = "status-index";
    public const string ChangeStatusIndexCmd = "change-status-index";

    public const string ListClassesCmd = "list-classes";    
    public const string GetClassCmd = "get-class";
    public const string AddUpdateClassCmd = "add-update-class";        

    public const string ListMethodsCmd = "list-class-methods";
    public const string GetClassMethodCmd = "get-class-method";
    public const string AddUpdateMethodCmd = "add-update-method";
            
    public const string ListTasksCmd = "list-pending-tasks";
    public const string GetNextTaskCmd = "get-next-pending-task";
    public const string MarkTaskStatusCmd = "mark-task-status";
    public const string AddUpdateTaskCmd = "add-task";
    
    
    public const string ListKnowledgeCmd = "list-knowledge";
    public const string AddKnowledgeCmd = "add-knowledge";
    public const string UpdateKnowledgeCmd = "update-knowledge";
    public const string DeleteKnowledgeCmd = "delete-knowledge";

    // Tool descriptions
    public const string ListProjectsDesc = "Gets list of available projects. A project is the configured name and the root folder allowed to access.";
    public const string ListFoldersDesc = "Lists the directories in the folder or root folder if a folder is not specified. Filter is c# SearchPattern empty defalut is *";
    public const string ListFilesDesc = "Lists the files in the folder or root folder if a folder is not specified. Filter is c# SearchPattern empty defalut is *";

    public const string GetFileDesc = "Gets the File and it's contents.";
    public const string InsertFileDesc = "Creates a file. Use to make a new file at the location in path parameter. Path is a reletive to the root path.";
    public const string UpdateFileDesc = "Updates a file. Use to update a file at the location in path parameter. Path is a reletive to the root path.";
    public const string DeleteFileDesc = "Deletes the file at the location identified in the path parameter. Path is a reletive to the root path.";

    public const string CreateFolderDesc = "Creates a new directory folder at location in the path parameter, pass true to createDirectories to confirm creation.";
    public const string DeleteFolderDesc = "Deletes a directory folder at location in the path parameter. pass true to deleteDirectories to confirm removal.";

    public const string ResyncIndexCmdDesc = "Resyncs the Index. The index will parse namespaces, classes, fields, events, and methods.  When a file changes watches should detect and reindex within 5 seconds after first change and will run 5 second intervals until queue is cleared, processing batch at a time.";
    public const string StatusIndexCmdDesc = "Gets the status of the index service. Returns Currnet Counts and if it's Enabled or not.";
    public const string ChangeStatusIndexCmdDesc = "Used to change the status of the index service. Enabled parameter, pass true to endable false to disable.  Active by default.";

    public const string ListClassesCmdDesc = "Lists namespaces, classes in the index service.";
    public const string GetClassCmdDesc = "Gets the class and its contents from the file ";
    public const string AddUpdateClassCmdDesc = "Adds or Updates a class. Use Get to retreive ClassContent object, modifiy and return here. ";    
    public const string DeleteClassCmdDesc = "Deletes class and its content from the namespace.";

    public const string ListClassMethodsCmdDesc = "Lists the methods for the specified class.";
    public const string GetClassMethodCmdDesc = "Gets the methods contents for the specified class.";
    public const string AddUpdateMethodCmdDesc = "Adds a method and it's contents to the specified class.";
    public const string UpdateClassMethodCmdDesc = "Updates method by splicing new method over top of old one identified in the get.";
    public const string DeleteMethodCmdDesc = "Deletes a method by name within the specified class.";

    public const string ListTasksCmdDesc = "lists the pending tasks stats";
    public const string GetNextTaskCmdDesc = "Get the next pending task.  Getting the task with expectation that a call to mark-task-status will follow.";
    public const string MarkTaskStatusCmdDesc = "Marks the status of the task. ";
    public const string AddTaskCmdDesc = "Add a task or sub task by naming the parent task and a description.";
    public const string UpdateTaskCmdDesc = "Update task description";
    public const string DeleteTaskCmdDesc = "Delete the task.";

    public const string ListKnowledgeCmdDesc = "List important knowledge any worker should read when working with daemonsmcp server.";
    public const string AddKnowledgeCmdDesc = "Adds a knowledge card to the list of important knowledge cards";
    public const string UpdateKnowledgeCmdDesc = "Update a existing knowledge card with new details";
    public const string DeleteKnowledgeCmdDesc = "Delete the identified knowledge card.";


    // Tool parameter descriptions
    public const string ProjectNameParamDesc = $"The configured project name to work in.";
    public const string FolderPathParamDesc = "Path to the directory. (relative to project root) If empty, the root of the project is used.";
    public const string FolderFilterParamDesc = "Filter for directories. (See Directory.GetDirectories SearchPattern) If empty, * is used.";
    public const string FilePathParamDesc = "Path to the file. (relative to project root)";
    public const string FileFilterParamDesc = "Filter for files. (See Directory.GetFiles SearchPattern specifically) If empty, * is used.";

    public const string FileContentParamDesc = "File content to write";
    public const string CreateFolderOptionDesc = "Option to confirm Create folders if it they don't exist; defaults to true";
    public const string DeleteFolderOptionDesc = "Option to confirm the removal; defaults to true";
    public const string OverwriteFileOptionDesc = "Overwrite file if it already exists";

    public const string ForceRebuildOptionDesc = "Force a complete rebuild of every file in the index, otherwise only the changes will be processed";
    public const string ChangeStatusIndexEnableDesc = "Enable or disable the indexer, pass true to enable false to disable";

    public const string ClassFilterParamDesc = "Filter classes by name uses string.Contains in filter";
    public const string MethodFilterParamDesc = "Filter methods by name uses string.Comtains in filter ";
    public const string PageNoParamDesc ="The page number to take results from default is 1.";
    public const string ItemsPerPageParamDesc = "The max number of items to take in this call.";
    public const string NamespaceFilterParamDesc = "Filter namespaces by name uses string.Contains as filter";

    public const string ClassIdParamDesc = "The int Id of the IndexClassItem or ClassId in ClassListing.";

    public const string ClassContentParamDesc = "The ClassContent object returned from GetClass or a modified version to update.";
    public const string MethodNameParamDesc = "The name of the method to add, update, or delete.";
    public const string MethodContentParamDesc = "The MethodContent object returned from GetClass or a modified version to update.";
    public const string MethodIdParamDesc = "The int ID of the Method in the Methods table.";


    // Debugging prefixes
    public const string Dd0 = "[DaemonsMCP][Debug]";
    public const string Dd1 = "[DaemonsMCP][ProjectTool] ";
    public const string Dd2 = "[DaemonsMCP][Security] ";

    // Error Messages
    public const string InvalidSkippingProject = "Skipping invalid project configuration";
    public const string ErrorNoConfig = "AppConfig cannot be null";

    // Index Table and column names    
    public const string FileTbl = "Files";
    public const string FileIdCol = "Id";        
    public const string FilePathNameCol = "FilePathName";
    public const string FileSizeCol = "Size";    
    public const string FileModifiedCol = "Modified";

    public const string ClassesTbl = "Classes";
    public const string ClassesIdCol = "Id";
    public const string ClassesFileIdCol = "FilesId"; // foreign key to Files table
    public const string ClassesFileNameCol = "FileName";
    public const string ClassesNamespaceCol = "Namespace";
    public const string ClassesNameCol = "MethodName";
    public const string ClassesLineStartCol = "LineStart";
    public const string ClassesLineEndCol = "LineEnd";

    public const string MethodsTbl = "Methods";
    public const string MethodsIdCol = "Id";
    public const string MethodsClassIdCol = "ClassId";
    public const string MethodsNameCol = "MethodName";
    public const string MethodsReturnTypeCol = "ReturnType";
    public const string MethodsParametersCol = "Parameters"; // JSON string of parameters
    public const string MethodsLineStartCol = "LineStart";
    public const string MethodsLineEndCol = "LineEnd";

    public const string PropertiesTbl = "Properties";
    public const string PropertiesIdCol = "Id";
    public const string PropertiesClassIdCol = "ClassId";
    public const string PropertiesNameCol = "MethodName";
    public const string PropertiesTypeCol = "Type";
    public const string PropertiesLineStartCol = "LineStart";
    public const string PropertiesLineEndCol = "LineEnd";
    public const string PropertiesHasGetterCol = "HasGetter"; // boolean
    public const string PropertiesHasSetterCol = "HasSetter"; // boolean

    public const string EventsTbl = "Events";
    public const string EventsIdCol = "Id";
    public const string EventsClassIdCol = "ClassId";
    public const string EventsNameCol = "MethodName";
    public const string EventsTypeCol = "Type"; // Type of the event handler
    public const string EventsLineStartCol = "LineStart";
    public const string EventsLineEndCol = "LineEnd";

    // what's the difference between field and property?  property has getter/setter methods, field is a variable
    // public const string FieldsTbl = "Fields";

    /*
1. Fields(IFieldSymbol)	Variables declared directly in the class.
2. Properties(IPropertySymbol)	Properties with getters/setters.
3. Methods(IMethodSymbol)	Regular methods, constructors, destructors, operators, and static methods.
4. Events(IEventSymbol)	Events declared in the class.
5. Indexers(IPropertySymbol with IsIndexer == true)	Special properties that allow array-like access.
6. Nested Types (INamedTypeSymbol)	Classes, structs, enums, interfaces, or delegates declared within the class.
7. Type Parameters(ITypeParameterSymbol)	If the class is generic.
8. Attributes(AttributeData)	Attributes applied to the class or its members.
9. Base Types and Interfaces	The class’s base type and implemented interfaces.

see GetLeadingTrivia() for comments above attributes example:
var start = method.GetLeadingTrivia().FirstOrDefault()?.SpanStart ?? method.SpanStart;
var end = method.Span.End;
var textChunk = sourceText.ToString(TextSpan.FromBounds(start, end));

    */

  }
}
