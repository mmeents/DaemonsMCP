# DaemonsMCP Configuration Guide

Complete guide to configuring DaemonsMCP V2 for security, performance, and project management.

## Configuration File Location

DaemonsMCP uses `daemonsmcp.json` in the main application directory:
```
DaemonsMCP/DaemonsMCP/daemonsmcp.json
```

## Complete Configuration Schema

```json
{
  "version": "2.0",
  "daemon": {
    "name": "DaemonMCP",
    "version": "2.0.0"
  },
  "projects": [
    {
      "name": "ProjectName",
      "description": "Project description",
      "path": "C:\\absolute\\path\\to\\project",
      "enabled": true
    }
  ],
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".js", ".py", ".md", ".txt", ".json", ".xml", ".yml", ".yaml"],
    "blockedExtensions": [".exe", ".dll", ".bat", ".cmd", ".sh", ".ps1"],
    "maxFileSize": "10MB",
    "maxFileWriteSize": "5MB",
    "writeProtectedPaths": [".git", ".vs", "bin", "obj", "node_modules", "packages"]
  },
 
}
```

## Project Configuration

### Basic Project Settings

```json
{
  "name": "MyProject",
  "description": "Description shown in project listings",
  "path": "C:\\absolute\\path\\to\\project",
  "enabled": true
}
```

**Parameters:**
- `name` (required): Unique identifier used in tool calls
- `description` (required): Human-readable description
- `path` (required): Absolute path to project root directory
- `enabled` (optional): Enable/disable project access (default: true)

### Path Configuration Best Practices

**✅ Recommended:**
```json
"path": "C:/projects/MyApp"
"path": "C:\\projects\\MyApp"
"path": "/home/user/projects/MyApp"
```

**❌ Avoid:**
```json
"path": "..\\MyApp"              // Relative paths
"path": "C:MyApp"                // Missing separator  
"path": "\\\\server\\share"      // UNC paths (untested)
```

### Multiple Projects Example

```json
"projects": [
  {
    "name": "MainApp",
    "description": "Primary application codebase",
    "path": "C:/development/MainApp",
    "enabled": true
  },
  {
    "name": "SharedLibrary", 
    "description": "Common utilities and models",
    "path": "C:/development/SharedLibrary",
    "enabled": true
  },
  {
    "name": "TestProject",
    "description": "Experimental features (disabled)",
    "path": "C:/development/TestProject", 
    "enabled": false
  }
]
```

## Security Configuration

### Write Operations Control

```json
"security": {
  "allowWrite": true  // Master switch for all write operations
}
```

**Settings:**
- `true`: Enable create, update, delete operations
- `false`: Read-only mode (blocks all modifications)

### File Extension Filtering

```json
"allowedExtensions": [
  ".cs", ".vb",           // .NET source
  ".js", ".ts", ".jsx",   // JavaScript/TypeScript
  ".py", ".rb", ".php",   // Other languages
  ".md", ".txt", ".rst",  // Documentation
  ".json", ".xml", ".yml", ".yaml", // Configuration
  ".sql", ".html", ".css" // Data and web
],
"blockedExtensions": [
  ".exe", ".dll", ".so",  // Executables and libraries
  ".bat", ".cmd", ".sh",  // Scripts
  ".ps1", ".msi",         // Windows-specific
  ".zip", ".tar", ".gz"   // Archives
]
```

**Processing Order:**
1. Check if extension is in `blockedExtensions` → **Block**
2. Check if extension is in `allowedExtensions` → **Allow**  
3. If neither list contains extension → **Block** (whitelist approach)

### File Size Limits

```json
"maxFileSize": "10MB",      // Maximum file size for reading
"maxFileWriteSize": "5MB"   // Maximum content size for writing
```

**Supported Units:**
- `KB`, `MB`, `GB` (case-insensitive)
- Numbers without units are treated as bytes
- Examples: `"1024"`, `"500KB"`, `"2MB"`, `"1GB"`

### Write-Protected Paths

```json
"writeProtectedPaths": [
  ".git",          // Git repository data
  ".vs",           // Visual Studio files  
  ".vscode",       // VS Code settings
  "bin",           // Compiled binaries
  "obj",           // Build intermediates
  "node_modules",  // Node.js dependencies
  "packages",      // NuGet packages
  "target",        // Java build output
  "__pycache__"    // Python cache
]
```

**Protection Rules:**
- Blocks **any write operation** to these paths
- Applies to files **within** these directories
- Relative to each project root
- Case-sensitive matching

## Security Profiles

### Development Profile (Permissive)
```json
"security": {
  "allowWrite": true,
  "allowedExtensions": [".cs", ".js", ".py", ".md", ".txt", ".json", ".xml", ".yml", ".yaml", ".html", ".css", ".sql"],
  "blockedExtensions": [".exe", ".dll"],
  "maxFileSize": "50MB",
  "maxFileWriteSize": "10MB", 
  "writeProtectedPaths": [".git", "bin", "obj"]
}
```

### Production Profile (Restrictive)
```json
"security": {
  "allowWrite": false,
  "allowedExtensions": [".cs", ".md", ".txt", ".json"],
  "blockedExtensions": [".exe", ".dll", ".bat", ".cmd", ".sh", ".ps1", ".msi"],
  "maxFileSize": "5MB",
  "maxFileWriteSize": "1MB",
  "writeProtectedPaths": [".git", ".vs", ".vscode", "bin", "obj", "node_modules", "packages"]
}
```

### Documentation Profile (Read-Only)
```json
"security": {
  "allowWrite": false,
  "allowedExtensions": [".md", ".txt", ".rst", ".html"],
  "blockedExtensions": ["*"],
  "maxFileSize": "10MB",
  "maxFileWriteSize": "0",
  "writeProtectedPaths": ["*"]
}
```

## Indexing Configuration

### Basic Indexing Settings

Indexing is build in.

### Index Storage Structure

For each project, indexing creates:
```
ProjectRoot/
├── .daemons/
│   ├── ProjectName.pktbs         // Files, Class definitions and Methods 
└── ProjectFiles...
```

## Logging Configuration

Logging is configured to output to the Applications Data folder in a folder named logs.  you could update the method in Extensions folder in Core Sx.cs AppDataFolder method.  

### Log Levels

- This still need work is configured for debug level by default.


## Environment-Specific Configurations

### Windows Development
```json
{
  "version": "2.0",
  "projects": [
    {
      "name": "MainProject",
      "path": "C:/Development/MainProject",
      "enabled": true
    }
  ],
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".js", ".md", ".json", ".xml"],
    "writeProtectedPaths": [".git", ".vs", "bin", "obj"]
  }
}
```

### Linux/macOS Development  
```json
{
  "version": "2.0",
  "projects": [
    {
      "name": "MainProject", 
      "path": "/home/user/projects/MainProject",
      "enabled": true
    }
  ],
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".py", ".md", ".json", ".yml"],
    "writeProtectedPaths": [".git", "bin", "obj", "__pycache__"]
  }
}
```

## Configuration Validation

DaemonsMCP validates configuration on startup:

### Required Fields
- `version` must be "2.0"
- Each project must have `name`, `description`, and `path`
- `security.allowedExtensions` must be an array

### Path Validation
- Project paths must exist and be accessible
- Index paths will be created if they don't exist
- Write operations validate against security settings

### Error Examples

**Invalid project path:**
```
Error: Project 'MyProject' path does not exist: C:\NonExistent\Path
```

**Invalid file extension:**
```
Error: File extension '.exe' is blocked by security configuration
```

**Invalid size format:**
```
Error: Invalid size format '10XB'. Use formats like '10MB', '500KB', '1GB'
```


## Troubleshooting Configuration

### Test Configuration Validity
```bash
# Run in test mode to validate config
dotnet run --configuration Release -- --validate-config
```

### Common Configuration Errors

**1. Path Issues:**
```
Solution: Use absolute paths with forward slashes or escaped backslashes
```

**2. Permission Errors:**
```
Solution: Ensure read/write access to project and index directories
```

**3. Extension Conflicts:**
```
Solution: Ensure allowedExtensions and blockedExtensions don't conflict
```

**4. Size Format Errors:**
```
Solution: Use format "NumberUnit" like "10MB", "500KB"
```

## Configuration Management

### Environment Variables
Override config file settings with environment variables:
```bash
DAEMONSMCP_SECURITY_ALLOWWRITE=false
DAEMONSMCP_LOGGING_LEVEL=Debug
```

### Multiple Configuration Files
Use different configs for different environments:
```bash
dotnet run -- --config=daemonsmcp.production.json
dotnet run -- --config=daemonsmcp.development.json
```

### Configuration Backup
Always backup your working configuration:
```bash
cp daemonsmcp.json daemonsmcp.backup.json
```

## Security Best Practices

1. **maxFileSize** it's important to guard your token usage. let more in as needed, 50KB is current max.
2. **Use Repository** and tools like Visual Studio to manage code changes
3. **Regularly review** allowed extensions and paths. Obj hits too often.
4. **Monitor logs** for problems, code is new and edges are many. 2 locations your clients logs and DaemonsMCP app folder (ex: c:\ProgramData\DaemonsMCP on Windows)
5. **Keep backups** of important configurations, cleanup old files often.
6. **Test changes** in development environment first
7. **Document** project-specific security requirements

## Next Steps

- Review [Tool Reference](TOOLS.md) for complete API documentation
- Check [Troubleshooting Guide](TROUBLESHOOTING.md) for common issues
- See [Architecture Overview](ARCHITECTURE.md) for implementation details
