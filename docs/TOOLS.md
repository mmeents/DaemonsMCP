# DaemonsMCP Tool Reference

Complete reference for all DaemonsMCP V2.4.0 tools with parameters, examples, and usage patterns.

## Documentation & Metadata

### `daemonsmcp:readme`
Gets living documentation from the DaemonsMCP project's internal node structure.

**Parameters:** None

**Response:**
Returns hierarchical node structure containing project documentation, architecture notes, and implementation details.

**Notes:**
- Documentation stored as nodes in `Storage.pktbs`
- Structured hierarchically for easy navigation
- Updated continuously as project evolves
- Includes architecture details, data flow, and critical implementation notes

**Example:**
```
Read the DaemonsMCP documentation to understand how it works
```

---

## File & Directory Operations

### `daemonsmcp:list-projects`
Lists all configured projects with metadata.

**Parameters:** None

**Response:**
```json
{
  \"projects\": [
    {
      \"Id\": 1,
      \"Name\": \"ProjectName\",
      \"Description\": \"Project description\", 
      \"Path\": \"C:\\\\path\\\	o\\\\project\",
      \"IndexPath\": \"C:\\\\ProgramData\\\\DaemonsMCP\"
    }
  ]
}
```

**Notes:**
- V2.4.0 uses centralized IndexPath in ProgramData/DaemonsMCP
- Projects configured via DaemonsConfigViewer or stored in Projects.pktbs
- All projects share same index location for efficiency

**Example:**
```
What projects do you have access to?
```

---

### `daemonsmcp:list-project-files`
Lists files in a project directory with optional filtering.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (optional): Directory path relative to project root (default: root)
- `filter` (optional): File pattern filter (e.g., \"*.cs\", \"*.json\")

**Example:**
```
List all C# files in the DaemonsMCP/DaemonsMCP.Core directory
```

---

### `daemonsmcp:list-project-directories`
Lists directories in a project with optional filtering.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (optional): Directory path relative to project root (default: root)
- `filter` (optional): Directory pattern filter

**Example:**
```
Show me the directories in the DaemonsMCP project
```

---

### `daemonsmcp:get-project-file`
Reads file content with complete metadata.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): File path relative to project root

**Response:**
```json
{
  \"FileName\": \"example.cs\",
  \"Path\": \"path/to/example.cs\", 
  \"Size\": 1024,
  \"ContentType\": \"text/plain\",
  \"Encoding\": \"UTF-8\",
  \"Content\": \"file content here...\",
  \"IsBinary\": false
}
```

**Example:**
```
Show me the content of Program.cs in the DaemonsMCP project
```

---

### `daemonsmcp:create-project-file`
Creates a new file with specified content.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): File path relative to project root
- `content` (required): File content to write

**Security Notes:**
- File extension must be in `allowedExtensions` list (configured in Settings.pktbs)
- Path cannot be in `writeProtectedPaths`
- Content size must be under `maxFileWriteSize` limit
- Parent directories created automatically if needed

**Example:**
```
Create a new utility class called StringHelper.cs with basic string extension methods
```

---

### `daemonsmcp:update-project-file`
Updates an existing file with new content.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): File path relative to project root
- `content` (required): New file content

**Security Notes:**
- Automatic backup created before update
- Same security restrictions as create operations
- Backups stored in project's `.daemons/backups/` directory

**Example:**
```
Add a new method to the StringHelper.cs file for reversing strings
```

---

### `daemonsmcp:delete-project-file`
Deletes a file with safety confirmations.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): File path relative to project root

**Security Notes:**
- Automatic backup created before deletion
- Cannot delete files in `writeProtectedPaths`

**Example:**
```
Delete the test file I created earlier, but make sure to back it up first
```

---

### `daemonsmcp:create-project-directory`
Creates a new directory.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): Directory path relative to project root
- `createDirectories` (optional): Create parent directories if needed (default: true)

**Example:**
```
Create a new directory called \"Utilities\" in the project
```

---

### `daemonsmcp:delete-project-directory`
Deletes a directory with confirmation.

**Parameters:**
- `projectName` (required): The configured project name
- `path` (required): Directory path relative to project root
- `deleteDirectories` (optional): Confirm deletion (default: true)

**Security Notes:**
- Cannot delete directories in `writeProtectedPaths`
- Will delete recursively if directory contains files

**Example:**
```
Delete the empty test directory I created
```

---

## Code Intelligence Operations

### `daemonsmcp:resync-index`
Indexes and analyzes C# code structure across projects.

**Parameters:**
- `forceRebuild` (optional): Force complete rebuild vs incremental update (default: false)

**Notes:**
- Uses Microsoft.CodeAnalysis (Roslyn) for parsing
- Indexes classes, methods, namespaces, and relationships
- File watchers automatically trigger reindexing on changes (5-second debounce)
- Index stored in centralized location: `C:\\ProgramData\\DaemonsMCP\\{ProjectName}_index.pktbs`
- Background processing with queue management for efficiency

**Example:**
```
Index all the C# code in the projects and refresh the analysis
```

---

### `daemonsmcp:status-index`
Gets the current status of the indexing service.

**Parameters:** None

**Response:**
```json
{
  \"Enabled\": true,
  \"TotalClasses\": 45,
  \"TotalMethods\": 234,
  \"QueueCount\": 0,
  \"LastUpdate\": \"2025-10-05T20:30:15Z\"
}
```

**Notes:**
- `QueueCount` shows files waiting to be indexed
- Queue processed in batches every 5 seconds
- FileSystemWatcher monitors for changes automatically

**Example:**
```
What's the current status of the code indexing service?
```

---

### `daemonsmcp:change-status-index`
Enables or disables the indexing service.

**Parameters:**
- `enableIndex` (required): True to enable, false to disable

**Notes:**
- Disabling stops file watching and index updates
- Useful for bulk file operations to reduce overhead
- Re-enabling triggers incremental update

**Example:**
```
Disable the code indexing service temporarily
```

---

### `daemonsmcp:list-classes`
Lists indexed classes with pagination and filtering.

**Parameters:**
- `projectName` (optional): Filter by specific project
- `namespaceFilter` (optional): Filter by namespace (contains match)
- `classFilter` (optional): Filter by class name (contains match)
- `pageNo` (required): Page number (1-based)
- `itemsPerPage` (required): Items per page (recommended: 20)

**⚠️ Critical:** Always specify `pageNo` and `itemsPerPage` explicitly to avoid empty results.

**Response:**
```json
{
  \"classes\": [
    {
      \"ClassId\": 1,
      \"ClassName\": \"ProjectService\",
      \"Namespace\": \"DaemonsMCP.Core.Services\",
      \"FileNamePath\": \"DaemonsMCP.Core/Services/ProjectService.cs\"
    }
  ],
  \"totalCount\": 45,
  \"pageNo\": 1,
  \"itemsPerPage\": 20
}
```

**Example:**
```
List all classes in the DaemonsMCP project, show me the first 20 results
```

---

### `daemonsmcp:get-class`
Gets detailed class information and content.

**Parameters:**
- `projectName` (required): The configured project name
- `classId` (required): The class ID from list-classes results

**Response:**
```json
{
  \"ClassId\": 1,
  \"ClassName\": \"ProjectService\", 
  \"Namespace\": \"DaemonsMCP.Core.Services\",
  \"FileNamePath\": \"DaemonsMCP.Core/Services/ProjectService.cs\",
  \"Content\": \"complete class source code...\",
  \"UsesClauseAdd\": \"additional using statements if needed\"
}
```

**Notes:**
- Content includes complete class with XML documentation
- UsesClauseAdd suggests additional using statements for dependencies
- Uses smart boundary detection to capture entire class including docs

**Example:**
```
Show me the complete source code for the ProjectService class
```

---

### `daemonsmcp:list-class-methods`
Lists methods within classes with pagination and filtering.

**Parameters:**
- `projectName` (optional): Filter by specific project
- `namespaceFilter` (optional): Filter by namespace (contains match)
- `classFilter` (optional): Filter by class name (contains match)
- `methodFilter` (optional): Filter by method name (contains match)
- `pageNo` (required): Page number (1-based)
- `itemsPerPage` (required): Items per page (recommended: 20)

**⚠️ Critical:** Always specify `pageNo` and `itemsPerPage` explicitly to avoid empty results.

**Example:**
```
List all methods in classes containing \"Service\" in their name, first 20 results
```

---

### `daemonsmcp:get-class-method`
Gets detailed method implementation.

**Parameters:**
- `projectName` (required): The configured project name
- `methodId` (required): The method ID from list-class-methods results

**Response:**
```json
{
  \"MethodId\": 15,
  \"MethodName\": \"ValidateProjectPath\",
  \"ClassId\": 1,
  \"ClassName\": \"ProjectService\",
  \"Namespace\": \"DaemonsMCP.Core.Services\", 
  \"FileNamePath\": \"DaemonsMCP.Core/Services/ProjectService.cs\",
  \"Content\": \"complete method source code...\",
  \"UsesClauseAdd\": \"additional using statements if needed\"
}
```

**Example:**
```
Show me the implementation of the ValidateProjectPath method
```

---

### `daemonsmcp:add-update-class`
Adds a new class or updates an existing one.

**Parameters:**
- `projectName` (required): The configured project name
- `classContent` (required): ClassContent object with **PascalCase** properties:
  - `ClassId`: 0 for new class, existing ID for update
  - `ClassName`: Class name
  - `Namespace`: Target namespace
  - `FileNamePath`: File path relative to project root
  - `Content`: Complete class source code
  - `UsesClauseAdd`: Additional using statements

**⚠️ Critical:** Use **PascalCase** property names (ClassId, not classId) to avoid serialization errors.

**Notes:**
- Automatically creates file if it doesn't exist
- Respects security settings for file operations
- Triggers automatic reindexing via FileSystemWatcher

**Example:**
```
Create a new utility class called FileHelper in the Utilities namespace
```

---

### `daemonsmcp:add-update-method`
Adds a new method or updates an existing one within a class.

**Parameters:**
- `projectName` (required): The configured project name
- `methodContent` (required): MethodContent object with **PascalCase** properties:
  - `MethodId`: 0 for new method, existing ID for update
  - `ClassId`: Target class ID
  - `MethodName`: Method name
  - `ClassName`: Class name (for reference)
  - `Namespace`: Namespace (for reference)
  - `FileNamePath`: File path (for reference)
  - `Content`: Complete method source code
  - `UsesClauseAdd`: Additional using statements

**⚠️ Critical:** Use **PascalCase** property names to avoid serialization errors.

**Security Notes:**
- Automatic backup created before modification
- Syntax validation performed before insertion
- Method inserted within class boundaries

**Example:**
```
Add a new validation method to the SecurityService class
```

---

## Project Management Operations

### `daemonsmcp:list-nodes`
Lists hierarchical project nodes with filtering and depth control.

**Parameters:**
- `nodeId` (optional): Parent node ID to list children from
- `maxDepth` (optional): Maximum recursion depth (default: 1)
- `typeFilter` (optional): Filter by node type name (contains match)
- `statusFilter` (optional): Filter by status name (contains match)
- `nameContains` (optional): Filter by node name (contains match)
- `detailsContains` (optional): Filter by details content (contains match)

**Response:**
```json
[
  {
    \"Id\": 1,
    \"ParentId\": 0,
    \"Name\": \"Documentation Root\",
    \"Details\": \"Root documentation node\",
    \"TypeId\": 5,
    \"TypeName\": \"Documentation\",
    \"StatusId\": 10,
    \"Status\": \"Active\",
    \"Rank\": 1,
    \"Created\": \"2025-10-05T20:00:00Z\",
    \"Modified\": \"2025-10-05T20:00:00Z\",
    \"Completed\": null,
    \"Subnodes\": []
  }
]
```

**Notes:**
- Nodes stored in centralized `Storage.pktbs`
- Changes detected by RepositoryFileWatcher for hot reload
- DaemonsConfigViewer shows live updates when nodes change

**Example:**
```
Show me the project documentation structure with 2 levels of depth
```

---

### `daemonsmcp:get-nodes-by-id`
Gets a specific node and its subtree by ID.

**Parameters:**
- `nodeIds` (required): Node ID to retrieve

**Example:**
```
Get the details for node ID 5 and its children
```

---

### `daemonsmcp:add-update-nodes`
Creates or updates a node and its subtree.

**Parameters:**
- `node` (required): Node object with properties:
  - `Id`: 0 for new node, existing ID for update
  - `ParentId`: Parent node ID (0 for root)
  - `Name`: Node name
  - `Details`: Node description/content
  - `TypeId`: Node type ID
  - `StatusId`: Node status ID
  - `Rank`: Sort order within parent
  - `Subnodes`: Array of child nodes (optional)

**Notes:**
- Saves trigger RepositoryFileWatcher in DaemonsConfigViewer
- ConfigViewer shows reload prompt when changes detected
- Supports recursive node creation with subnodes

**Example:**
```
Create a new documentation section for the API reference
```

---

### `daemonsmcp:add-update-nodes-list`
Creates or updates multiple nodes in batch.

**Parameters:**
- `nodes` (required): Array of node objects

**Notes:**
- More efficient than individual updates for bulk operations
- Single save operation triggers one hot reload event

**Example:**
```
Create multiple todo items for the next sprint
```

---

### `daemonsmcp:remove-node`
Removes a node with configurable cascade behavior.

**Parameters:**
- `nodeId` (required): Node ID to remove
- `removeStrategy` (optional): How to handle children:
  - `PreventIfHasChildren` (default): Fail if node has children
  - `DeleteCascade`: Delete node and all children
  - `OrphanChildren`: Delete node, children become parentless
  - `ReparentToGrandparent`: Delete node, move children up one level

**Example:**
```
Delete the test documentation section and move its children to the parent
```

---

### `daemonsmcp:list-item-types`
Lists available node types for classification.

**Parameters:** None

**Response:**
```json
[
  {
    \"Id\": 1,
    \"Name\": \"Documentation\",
    \"Description\": \"Documentation nodes\",
    \"TypeEnum\": 1,
    \"ParentId\": 0
  }
]
```

**Notes:**
- Types stored in `Storage.pktbs`
- Configurable via DaemonsConfigViewer or this tool
- Supports hierarchical type structures

---

### `daemonsmcp:add-update-item-type`
Creates or updates node type definitions.

**Parameters:**
- `type` (required): ItemType object with Id, Name, Description, TypeEnum, ParentId

**Example:**
```
Create a new node type called \"Feature Request\"
```

---

### `daemonsmcp:list-status-types`
Lists available status types for nodes.

**Parameters:** None

**Response:**
```json
[
  {
    \"Id\": 1,
    \"Name\": \"Not Started\",
    \"Description\": \"Work not yet begun\",
    \"TypeEnum\": 1,
    \"ParentId\": 0
  }
]
```

---

### `daemonsmcp:add-update-status-type`
Creates or updates status type definitions.

**Parameters:**
- `status` (required): StatusType object with Id, Name, Description, TypeEnum, ParentId

**Example:**
```
Create a new status type called \"In Review\"
```

---

### `daemonsmcp:save-project-repo`
Manually saves the project repository data to disk.

**Parameters:** None

**Notes:**
- Normally called automatically after updates
- Use for manual checkpoint/backup operations
- Saves to `Storage.pktbs` in centralized location

---

## Todo Management Operations

### `daemonsmcp:make-todo-list`
Creates a todo list as a hierarchical node structure.

**Parameters:**
- `listName` (required): Name of the todo list
- `items` (required): Array of todo item names/descriptions

**Notes:**
- Creates or finds a todo list node by name
- Adds items as child nodes under the list
- Items start with \"Not Started\" status
- Lists stored under \"Todo Root\" node

**Example:**
```
Create a todo list called \"Sprint Planning\" with tasks for database design, API implementation, and testing
```

---

### `daemonsmcp:get-next-todo`
Gets the next available todo item and marks it as \"In Progress\".

**Parameters:**
- `listName` (required): Name of the todo list to search

**Response:**
```json
{
  \"Id\": 25,
  \"Name\": \"Implement user authentication\",
  \"Details\": \"Add OAuth2 integration with JWT tokens\",
  \"Status\": \"In Progress\",
  \"Started\": \"2025-10-05T22:30:00Z\"
}
```

**Notes:**
- Automatically updates status to \"In Progress\"
- Returns null if no pending items found
- Searches recursively through todo list tree

**Example:**
```
Get the next task from the \"Sprint Planning\" todo list
```

---

### `daemonsmcp:mark-todo-done`
Marks a todo item as completed.

**Parameters:**
- `itemId` (required): Todo item node ID

**Notes:**
- Sets status to \"Complete\"
- Records completion timestamp
- Item remains in list for tracking

**Example:**
```
Mark todo item 25 as completed
```

---

### `daemonsmcp:mark-todo-cancel`
Marks a todo item as cancelled.

**Parameters:**
- `itemId` (required): Todo item node ID

**Notes:**
- Sets status to \"Cancelled\"
- Records cancellation timestamp
- Item remains in list for tracking

**Example:**
```
Cancel todo item 30 - it's no longer needed
```

---

### `daemonsmcp:restore-todo`
Resets a todo item back to \"Not Started\" status.

**Parameters:**
- `itemId` (required): Todo item node ID

**Notes:**
- Clears completion/cancellation timestamp
- Useful for recurring tasks or when work needs redoing

**Example:**
```
Reset todo item 25 back to not started - we need to redo this task
```

---

## V2.4.0 New Features

### Hot Reload Support
All operations that modify `Storage.pktbs`, `Projects.pktbs`, or `Settings.pktbs` trigger RepositoryFileWatcher events:

- **DaemonsConfigViewer Detection**: Shows reload prompt when Claude makes changes
- **Bidirectional Updates**: Changes in ConfigViewer immediately available to DaemonsMCP
- **500ms Debounce**: Prevents excessive reloads during batch operations
- **Visual Indicators**: ConfigViewer shows when external changes detected

### Centralized Configuration
V2.4.0 moved from per-project `.daemons` folders to centralized storage:

**Previous (V2.x):**
```
ProjectRoot/.daemons/
├── index.pktbs
└── backups/
```

**Current (V2.4.0):**
```
C:\\ProgramData\\DaemonsMCP\\  (Windows)
~/.local/share/DaemonsMCP   (Linux/macOS)
├── Projects.pktbs          # All project configs
├── Settings.pktbs          # Security settings
├── Storage.pktbs           # All nodes/todos
└── ProjectName_index.pktbs # Code intelligence per project
```

**Benefits:**
- No repository pollution
- Single backup location
- Easy configuration migration
- Shared settings across projects

### PackedTables Integration
All configuration now uses PackedTables.NET binary format:

- **Faster**: 3x faster loading vs JSON
- **Smaller**: More efficient binary serialization
- **Structured**: Proper schema and relationships
- **Tooling**: Direct editing via PackedTables viewer

---

## Usage Patterns & Best Practices

### Working with Pagination
Always specify explicit pagination to avoid empty results:
```
List the first 20 classes in the project, page 1
```

### Code Modification Workflow
1. Use `list-classes` to find target class
2. Use `get-class` to examine current implementation  
3. Use `add-update-class` or `add-update-method` for changes
4. Backups are created automatically
5. Index updates automatically via FileSystemWatcher

### Project Organization
1. Use nodes for documentation and project structure
2. Use todo lists for task management
3. Use status and type systems for organization
4. Filter and search using contains matching
5. Leverage hot reload for cross-tool collaboration

### Security Considerations
- All write operations respect security configuration in `Settings.pktbs`
- Automatic backups protect against mistakes
- File type filtering prevents dangerous operations
- Path validation prevents directory traversal
- Size limits prevent context overflow and DoS

### Performance Tips
- Use appropriate page sizes (20 items recommended)
- Use filters to narrow results
- Index service runs automatically in background (5-second debounce)
- File watchers trigger incremental updates, not full rebuilds
- Disable indexing during bulk file operations for better performance

### Cross-Application Workflow
1. **Configure in DaemonsConfigViewer**: Add projects, set security
2. **Work with Claude**: Use DaemonsMCP tools to code and organize
3. **Review in ConfigViewer**: See changes, navigate node trees
4. **Hot Reload**: Both tools stay in sync automatically

---

## Troubleshooting

### Empty Results from list-classes or list-class-methods
**Problem:** Queries return empty arrays  
**Solution:** Always specify `pageNo` and `itemsPerPage` explicitly:
```
List classes in DaemonsMCP project, page 1, 20 items per page
```

### Write Operations Failing
**Problem:** File creation/updates rejected  
**Checks:**
1. File extension in `Settings.pktbs` allowedExtensions?
2. Path not in writeProtectedPaths?
3. Write operations enabled in Settings?
4. File size under maxFileWriteSize limit?

### Index Not Updating
**Problem:** Code changes not reflected in searches  
**Solutions:**
1. Check `status-index` - is indexer enabled?
2. Wait 5 seconds for debounce to complete
3. Use `resync-index` with `forceRebuild: true` for full refresh

### ConfigViewer Not Showing Changes
**Problem:** Claude made changes but ConfigViewer doesn't update  
**Solution:** Click the \"Reload\" button in ConfigViewer status bar when prompted

### Serialization Errors
**Problem:** \"Serialization failed\" errors  
**Solution:** Use PascalCase property names in method/class content objects (ClassId not classId)

---

## Migration from V2.x

If upgrading from DaemonsMCP V2.x:

1. **Backup your data**: Copy `.daemons` folders from all projects
2. **Run DaemonsConfigViewer**: Use \"Import\" feature to load old `daemonsmcp.json`
3. **Verify Projects**: Check all projects loaded correctly in ConfigViewer
4. **Update Claude Desktop config**: Point to new V2.4.0 executable
5. **Test**: Run `list-projects` to confirm configuration
6. **Clean up**: Old `.daemons` folders in projects can be deleted after verification

**Breaking Changes:**
- Configuration format changed from JSON to PackedTables
- Index location moved from project/.daemons to centralized ProgramData
- Manual reconfiguration required (import feature available)

---

## API Versioning

DaemonsMCP follows semantic versioning:
- **Major** (2): Breaking API changes
- **Minor** (4): New features, backward compatible  
- **Patch** (0): Bug fixes, performance improvements

V2.4.0 is backward compatible with V2.x tool calls but requires configuration migration.