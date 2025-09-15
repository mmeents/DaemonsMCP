# DaemonsMCP Architecture Overview

Deep dive into the technical architecture, design patterns, and implementation details of DaemonsMCP V2.

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      MCP Client                             │
│                   (Claude Desktop)                          │
└─────────────────────┬───────────────────────────────────────┘
                      │ JSON-RPC over stdio
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                  MCPSharp Framework                         │
│              (Transport & Protocol)                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│               DaemonsMCP Service                            │
│                                                             │
│  ┌─────────────────┐  ┌──────────────────────────────────┐  │
│  │ Hosted Service  │  │     Dependency Injection         │  │
│  │   Lifecycle     │  │         Container                │  │
│  └─────────────────┘  └──────────────────────────────────┘  │
│                                                             │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐  │
│  │ DaemonsTools    │  │ Service Layer   │  │ Data Layer  │  │
│  │  (MCP Bridge)   │  │                 │  │             │  │
│  └─────────────────┘  └─────────────────┘  └─────────────┘  │
└─────────────────────────────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                File System & Index Storage                  │
│           (PackedTables.NET + File System)                  │
└─────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. MCPSharp Framework Integration

**Purpose**: Handles MCP protocol communication and JSON-RPC transport.

**Key Features:**
- Automatic camelCase ↔ PascalCase conversion
- Attribute-based tool registration
- Duplex pipe communication vs basic stdio
- Built-in error handling and serialization

**Integration Point:**
```csharp
[McpTool(\"list-projects\")]
public async Task<ProjectListResponse> ListProjectsAsync()
{
    return await _projectService.GetProjectsAsync();
}
```

### 2. Dependency Injection Architecture

**Container Setup:**
```csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);

// Core services
builder.Services.AddSingleton<IProjectService, ProjectService>();
builder.Services.AddSingleton<IProjectFileService, ProjectFileService>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();
builder.Services.AddSingleton<IValidationService, ValidationService>();

// Index services
builder.Services.AddSingleton<IIndexService, IndexService>();
builder.Services.AddSingleton<IProjectIndexModel, ProjectIndexModel>();

// MCP integration
builder.Services.AddSingleton<DaemonsTools>();
builder.Services.AddHostedService<DaemonsMcpHostedService>();
```

**Benefits:**
- Clean separation of concerns
- Testable components
- Proper lifecycle management
- Easy to extend and modify

### 3. Service Layer Architecture

#### IProjectService
**Responsibilities:**
- Project configuration management
- Project validation and setup
- Multi-project coordination

```csharp
public interface IProjectService
{
    Task<IEnumerable<ProjectInfo>> GetProjectsAsync();
    Task<ProjectInfo> GetProjectAsync(string projectName);
    Task<bool> ValidateProjectAsync(string projectName);
}
```

#### IProjectFileService
**Responsibilities:**
- File CRUD operations
- Path validation and security
- MIME type detection and encoding

```csharp
public interface IProjectFileService
{
    Task<FileResponse> GetFileAsync(string projectName, string path);
    Task<FileResponse> CreateFileAsync(string projectName, string path, string content);
    Task<FileResponse> UpdateFileAsync(string projectName, string path, string content);
    Task<bool> DeleteFileAsync(string projectName, string path);
}
```

#### ISecurityService
**Responsibilities:**
- File extension validation
- Path traversal prevention
- Size limit enforcement
- Write protection checks

```csharp
public interface ISecurityService
{
    bool IsFileExtensionAllowed(string extension);
    bool IsPathWriteProtected(string path);
    bool ValidateFileSize(long size, bool isWrite = false);
    bool ValidatePathSafety(string path);
}
```

### 4. Code Intelligence System

#### IndexService Architecture
```
┌─────────────────────────────────────────────┐
│              IndexService                   │
├─────────────────────────────────────────────┤
│ • File Watcher Integration                  │
│ • Microsoft.CodeAnalysis Integration       │
│ • Batch Processing Queue                    │
│ • Background Processing                     │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│         ProjectIndexModel                   │
├─────────────────────────────────────────────┤
│ • PackedTables.NET Integration              │
│ • Class/Method/Namespace Storage            │
│ • Query Processing                          │
│ • Pagination Support                        │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│         PackedTables Storage                │
├─────────────────────────────────────────────┤
│ • ProjectName.pktbs                         │
│   • File, Classes and Methods tables        │
│ • Storage.pktbs (Nodes and Types tables)    │
└─────────────────────────────────────────────┘
```

#### Code Analysis Pipeline
1. **File Detection**: File system watchers detect .cs file changes
2. **Queue Management**: Changes queued for batch processing
3. **AST Parsing**: Microsoft.CodeAnalysis parses syntax trees
4. **Data Extraction**: Classes, methods, namespaces extracted
5. **Storage Update**: PackedTables.NET stores structured data
6. **Index Update**: Search indexes updated for queries

### 5. Project Management System

#### Node Hierarchy Design
```
Root Node (Id: 0)
├── Documentation (Type: Documentation, Status: Active)
│   ├── API Reference (Type: Documentation, Status: Complete)
│   └── User Guide (Type: Documentation, Status: In Progress)
├── Todo Root (Type: Container, Status: Active)
│   ├── Sprint 1 (Type: Todo, Status: Active)
│   │   ├── Feature A (Type: Task, Status: Complete)
│   │   └── Feature B (Type: Task, Status: In Progress)
│   └── Bug Fixes (Type: Todo, Status: Active)
└── Project Structure (Type: Container, Status: Active)
```

#### Node Properties
```csharp
public class Node
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public int StatusId { get; set; }
    public string Status { get; set; }
    public int Rank { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public DateTime? Completed { get; set; }
    public List<Node> Subnodes { get; set; }
}
```

## Data Storage Architecture

### PackedTables.NET Integration

**File Structure:**
```
ProjectRoot/
├── .daemons/
│   ├── ProjectName.pktbs     # Index Table set for code analysis
│   ├── nodes.pktbs           # Project management nodes can be configured anywhere
└── Project Files...
```

**Benefits of PackedTables.NET:**
- High-performance binary storage
- Structured data with indexing
- Cross-platform compatibility
- Minimal dependencies
- Efficient queries and updates

### Backup System Design

**Automatic Backup Strategy:**
1. **Pre-Operation Backup**: Created before any destructive operation
2. **Timestamped Names**: `filename.backup.yyyyMMdd_HHmmss` format
3. **Retention Policy**: Configurable maximum backup count
4. **Storage Location**: same directory original file is at.

**Backup Triggers:**
- File updates (via `update-project-file`)
- File deletions (via `delete-project-file`)
- Class modifications (via `add-update-class`)
- Method modifications (via `add-update-method`)

## Security Architecture

### Multi-Layer Security Model

```
┌─────────────────────────────────────────────┐
│            Request Input                    │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│         Input Validation                    │
│ • Parameter validation                      │
│ • Path sanitization                         │
│ • Content size checks                       │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│       Security Service Layer                │
│ • Extension whitelist/blacklist             │
│ • Path traversal prevention                 │
│ • Write protection checks                   │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│       Project Boundary Validation           │
│ • Project existence verification            │
│ • Path within project bounds                │
│ • Permission checks                         │
└─────────────┬───────────────────────────────┘
              │
┌─────────────▼───────────────────────────────┐
│         File System Operation               │
└─────────────────────────────────────────────┘
```

### Security Validation Flow

**1. Input Validation:**
```csharp
public class ValidationService : IValidationService
{
    public bool ValidateProjectName(string projectName)
    {
        return !string.IsNullOrWhiteSpace(projectName) && 
               projectName.Length <= 100 &&
               !projectName.Contains(\"..\");
    }
    
    public bool ValidatePath(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
               !Path.IsPathRooted(path) &&
               !path.Contains(\"..\");
    }
}
```

**2. Security Enforcement:**
```csharp
public class SecurityService : ISecurityService
{
    public bool IsFileExtensionAllowed(string extension)
    {
        if (_config.Security.BlockedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return false;
            
        return _config.Security.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
    
    public bool IsPathWriteProtected(string path)
    {
        return _config.Security.WriteProtectedPaths.Any(protected => 
            path.StartsWith(protected, StringComparison.OrdinalIgnoreCase));
    }
}
```

## Performance Architecture

### Indexing Performance

**Background Processing:**
- File changes queued for batch processing
- Configurable batch sizes for different project sizes
- Background service processes queue without blocking MCP operations

**File Watching Strategy:**
```csharp
public class IndexService : IIndexService
{
    private readonly FileSystemWatcher _watcher;
    private readonly Timer _batchTimer;
    private readonly ConcurrentQueue<string> _fileQueue;
    
    // Batch processing prevents overwhelming the system
    private void ProcessBatch()
    {
        var filesToProcess = new List<string>();
        while (filesToProcess.Count < _batchSize && _fileQueue.TryDequeue(out var file))
        {
            filesToProcess.Add(file);
        }
        
        if (filesToProcess.Any())
        {
            await ProcessFilesAsync(filesToProcess);
        }
    }
}
```

### Memory Management

**Large File Handling:**
- Stream-based file reading for large files
- Configurable size limits prevent memory exhaustion
- Early validation before loading content

**Index Caching:**
- PackedTables.NET provides efficient binary storage
- Minimal memory footprint for index data
- On-demand loading of class/method content

## Error Handling Architecture

### Structured Error Responses

**Error Response Format:**
```csharp
public class OperationResult<T>
{
    public bool Success { get; set; }
    public string Operation { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
    public string Exception { get; set; }
}
```

**Error Categories:**
1. **Validation Errors**: Input parameter issues
2. **Security Errors**: Permission or safety violations
3. **System Errors**: File system or configuration problems
4. **Processing Errors**: Code analysis or indexing failures

### Exception Handling Strategy

**Service Layer:**
```csharp
public async Task<FileResponse> GetFileAsync(string projectName, string path)
{
    try
    {
        // Validation
        if (!_validationService.ValidateProjectName(projectName))
            return FileResponse.Error(\"Invalid project name\");
            
        // Security checks
        if (!_securityService.IsFileExtensionAllowed(extension))
            return FileResponse.Error(\"File extension not allowed\");
            
        // Operation
        var content = await File.ReadAllTextAsync(fullPath);
        return FileResponse.Success(content);
    }
    catch (UnauthorizedAccessException ex)
    {
        return FileResponse.Error($\"Access denied: {ex.Message}\");
    }
    catch (FileNotFoundException ex)
    {
        return FileResponse.Error($\"File not found: {ex.Message}\");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, \"Unexpected error reading file\");
        return FileResponse.Error(\"Internal error occurred\");
    }
}
```

## Extension Points

### Adding New Tools

**1. Define Tool Method:**
```csharp
[McpTool(\"daemonsmcp:my-new-tool\")]
public async Task<MyResponse> MyNewToolAsync(
    [McpToolParam(\"projectName\")] string projectName,
    [McpToolParam(\"customParam\")] string customParam)
{
    // Implementation
}
```

**2. Add Service Dependencies:**
```csharp
// Register in Program.cs
builder.Services.AddSingleton<IMyNewService, MyNewService>();
```

**3. Implement Business Logic:**
```csharp
public class MyNewService : IMyNewService
{
    public async Task<Result> ProcessAsync(string input)
    {
        // Business logic here
    }
}
```

### Adding New Security Policies

**Custom Security Service:**
```csharp
public class CustomSecurityService : SecurityService
{
    public override bool ValidateCustomRule(string input)
    {
        // Custom validation logic
        return base.ValidateCustomRule(input) && MyCustomCheck(input);
    }
}
```

### Adding New Storage Providers

**Storage Abstraction:**
```csharp
public interface IStorageProvider
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
    Task DeleteAsync(string key);
}

// Implement for different storage backends
public class PackedTablesStorageProvider : IStorageProvider { }
public class SqliteStorageProvider : IStorageProvider { }
```

## Testing Architecture

### Unit Testing Strategy

**Service Testing:**
```csharp
[Test]
public async Task GetFileAsync_ValidInput_ReturnsContent()
{
    // Arrange
    var mockValidation = new Mock<IValidationService>();
    var mockSecurity = new Mock<ISecurityService>();
    mockValidation.Setup(x => x.ValidateProjectName(\"test\")).Returns(true);
    mockSecurity.Setup(x => x.IsFileExtensionAllowed(\".txt\")).Returns(true);
    
    var service = new ProjectFileService(mockValidation.Object, mockSecurity.Object);
    
    // Act
    var result = await service.GetFileAsync(\"test\", \"file.txt\");
    
    // Assert
    Assert.IsTrue(result.Success);
}
```

### Integration Testing

**MCP Tool Testing:**
```csharp
[Test]
public async Task ListProjects_Integration_ReturnsProjects()
{
    // Arrange - setup test configuration
    var config = CreateTestConfiguration();
    var services = CreateTestServices(config);
    var tools = new DaemonsTools(services);
    
    // Act
    var result = await tools.ListProjectsAsync();
    
    // Assert
    Assert.IsNotNull(result.Projects);
    Assert.IsTrue(result.Projects.Any());
}
```

## Deployment Architecture

### Single Executable Deployment

**Self-Contained Build:**
```bash
dotnet publish --configuration Release --self-contained --runtime win-x64
```

**Benefits:**
- No .NET runtime dependency
- Single file deployment
- Platform-specific optimization

### Configuration Management

**Environment-Specific Configs:**
```
DaemonsMCP/
├── daemonsmcp.json              # Default configuration
├── daemonsmcp.development.json  # Development overrides
├── daemonsmcp.production.json   # Production overrides
└── appsettings.json            # .NET configuration
```

**Configuration Loading:**
```csharp
// Support multiple config files
var configBuilder = new ConfigurationBuilder()
    .AddJsonFile(\"daemonsmcp.json\", optional: false)
    .AddJsonFile($\"daemonsmcp.{environment}.json\", optional: true)
    .AddEnvironmentVariables(\"DAEMONSMCP_\");
```

## Monitoring and Observability

### Logging Strategy

**Structured Logging:**
```csharp
public class ProjectFileService
{
    private readonly ILogger<ProjectFileService> _logger;
    
    public async Task<FileResponse> GetFileAsync(string projectName, string path)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            [\"ProjectName\"] = projectName,
            [\"FilePath\"] = path,
            [\"Operation\"] = \"GetFile\"
        });
        
        _logger.LogInformation(\"Starting file read operation\");
        
        try
        {
            // Operation logic
            _logger.LogInformation(\"File read completed successfully\");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"File read operation failed\");
            throw;
        }
    }
}
```

### Performance Metrics

**Key Metrics to Monitor:**
- File operation response times
- Index processing queue length
- Memory usage during large operations
- Error rates by operation type
- Project access patterns

### Health Checks

**Service Health Monitoring:**
```csharp
public class DaemonsMcpHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            // Check index service status
            var indexStatus = await _indexService.GetStatusAsync();
            if (!indexStatus.Enabled)
                return HealthCheckResult.Degraded(\"Index service disabled\");
                
            // Check project accessibility
            var projects = await _projectService.GetProjectsAsync();
            if (!projects.Any())
                return HealthCheckResult.Unhealthy(\"No projects configured\");
                
            return HealthCheckResult.Healthy(\"All systems operational\");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(\"Health check failed\", ex);
        }
    }
}
```

## Future Architecture Considerations


### Integration Expansion

**2. Build System Integration:**
- MSBuild integration
- Dependency analysis
- Build artifact management

**3. IDE Integration:**
- Language server protocol support
- Real-time code intelligence
- Refactoring tool integration

This architecture provides a solid foundation for current needs while maintaining flexibility for future enhancements and scalability requirements.
