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
            
    public const string GetItemTypesCmd = "list-item-types";
    public const string GetStatusTypesCmd = "list-status-types";
    public const string AddUpdateItemTypeCmd = "add-update-item-type";
    public const string AddUpdateStatusTypeCmd = "add-update-status-type";
        
    public const string GetNodesCmd = "list-nodes";
    public const string GetNodesByIdCmd = "get-nodes-by-id";
    public const string AddUpdateNodesCmd = "add-update-nodes";
    public const string AddUpdateNodesListCmd = "add-update-nodes-list";

    public const string RemoveNodeCmd = "remove-node";
    
    public const string SaveProjectRepoCmd = "save-project-repo";


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

    public const string GetItemTypesCmdDesc = "Lists all the types available for Nodes object. Pass object into add-update-item-type to modify. ";
    public const string GetStatusTypesCmdDesc = "List all the status types available for Nodes objects. Pass objects into add-update-status-type to modify.";
    public const string AddUpdateItemTypeCmdDesc = "Add update item type, use to update types listed by list-item-type. use id=0 to add new.";
    public const string AddUpdateStatusTypeCmdDesc = "Add update status type, use to update status types listed by list-status-types. use id=0 to add new.";

    public const string GetNodesCmdDesc = "List nodes command searches for item nodes recursivly maxDepth deep.  item nodes are trees of types and status as listed in each. use add-update-nodes to add or update the tree nodes. ";
    public const string GetNodesByIdCmdDesc = "Get nodes by id, allows you to grab 1 tree recursiv by id maxLevels deep.";
    public const string AddUpdateNodesCmdDesc = "Add update nodes command adds or updates depending on tree passed in.  if it has id non zero it tries to update otherwise it tries to add. Recursive adds updates all nodes passed in. ";

    public const string AddUpdateNodesListCmdDesc = "Add update nodes list command adds or updates depending on list of trees passed in.  if it has id non zero it tries to update otherwise it tries to add. Recursive adds updates all nodes passed in. ";
    public const string RemoveNodeCmdDesc = "Remove a node from the tree. Strategy options: PreventIfHasChildren (default), DeleteCascade, OrphanChildren, ReparentToGrandparent";    

    public const string SaveProjectRepoCmdDesc = "Save project repo command writes Items and Types tables to disk. Should be needed as any update inherently calls save.";


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

    public const string NodeIdParamDesc = "The int Id of the node to get the tree for.";
    public const string MaxDepthParamDesc = "The max depth to recurse when listing nodes, default is 1, meaning get the nodes children. 2 would get children and theirs. 0 will skip children.";
    public const string StatusFilterParamDesc = "Filter nodes by status type name, uses string.Contains c# type filtering or null for all.";
    public const string TypeFilterParamDesc = "Filter nodes by item type name, uses string.Contains c# type filtering or null for all.";
    public const string NameContainsParamDesc = "Filter nodes by name, uses string.Contains c# type filtering or null for all.";
    public const string DetailsContainsParamDesc = "Filter nodes by details, uses string.Contains c# type filtering or null for all.";
    public const string NodesTreeParamDesc = "The Nodes tree object to add or update.";
    public const string ItemTypeParamDesc = "The ItemType object to add or update, result record from get-item-type call.";
    public const string StatusTypeParamDesc = "The StatusType object to add or update, result record from get-status-type call.";
    public const string NodesListTreeParamDesc = "The List<Nodes> tree objects to add or update.";
    public const string RemoveStrategyParamDesc = "How to handle child nodes: PreventIfHasChildren, DeleteCascade, OrphanChildren, or ReparentToGrandparent";

    // Nodes Status and Type defaults
    public const string TypeNone = "None";
    public const string TypeInternalRoot = "Internal Categories";
    public const string TypeStatusTypes = "Status Types";
    public const string TypeItemTypes = "Item Types";

    // Debugging prefixes
    public const string Dd0 = "[DaemonsMCP][Debug]";
    public const string Dd1 = "[DaemonsMCP][ProjectTool] ";
    public const string Dd2 = "[DaemonsMCP][Security] ";

    // Error Messages
    public const string InvalidSkippingProject = "Skipping invalid project configuration";
    public const string ErrorNoConfig = "AppConfig cannot be null";    
    public static string ClassServiceErrorMsg(Exception ex) => $"Error: {ex.Message}. Please Stop and report the error to end user.";

    // Storage and Process Management
    public const string TypesTbl = "Types";
    public const string TypeIdCol = "Id";
    public const string TypeParentCol = "TypeId"; // self-referencing foreign key
    public const string TypeEnumCol = "TypeEnum";
    public const string TypeNameCol = "TypeName";
    public const string TypeDescriptionCol = "Description";

    public const string ItemsTbl = "Items";
    public const string ItemIdCol = "Id";         // primary key   
    public const string ItemParentCol = "ParentId"; // self-referencing foreign key
    public const string ItemTypeIdCol = "TypeId"; // foreign key to Types table e.g. Bug, Feature, Task
    public const string ItemStatusCol = "StatusTypeId"; // e.g. Pending, InProgress, Completed        
    public const string ItemRankCol = "Rank";     // for ordering items of same parent
    public const string ItemNameCol = "ItemName";
    public const string ItemDetailsCol = "Details";    
    public const string ItemCreatedCol = "Created"; // DateTime
    public const string ItemModifiedCol = "Modified"; // DateTime
    public const string ItemCompletedCol = "Completed"; // DateTime?


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
    public const string ClassesNameCol = "Name";
    public const string ClassesLineStartCol = "LineStart";
    public const string ClassesLineEndCol = "LineEnd";

    public const string MethodsTbl = "Methods";
    public const string MethodsIdCol = "Id";
    public const string MethodsClassIdCol = "ClassId";
    public const string MethodsNameCol = "Name";
    public const string MethodsReturnTypeCol = "ReturnType";
    public const string MethodsParametersCol = "Parameters"; // JSON string of parameters
    public const string MethodsLineStartCol = "LineStart";
    public const string MethodsLineEndCol = "LineEnd";

    public const string PropertiesTbl = "Properties";
    public const string PropertiesIdCol = "Id";
    public const string PropertiesClassIdCol = "ClassId";
    public const string PropertiesNameCol = "Name";
    public const string PropertiesTypeCol = "Type";
    public const string PropertiesLineStartCol = "LineStart";
    public const string PropertiesLineEndCol = "LineEnd";
    public const string PropertiesHasGetterCol = "HasGetter"; // boolean
    public const string PropertiesHasSetterCol = "HasSetter"; // boolean

    public const string EventsTbl = "Events";
    public const string EventsIdCol = "Id";
    public const string EventsClassIdCol = "ClassId";
    public const string EventsNameCol = "Name";
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
