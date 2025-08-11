# DaemonsMCP

**Give LLMs hands-on control of your codebase** 👁️‍🗨️✋

DaemonsMCP is a comprehensive C# MCP (Model Context Protocol) service that provides LLMs with secure, full-featured access to explore, read, and **write** to local codebases. Built on MCPSharp for reliable transport and JSON-RPC communication, it gives your AI assistant the ability to see, navigate, understand, and **modify** your project files just like a developer would.

✅ **V2 - Enterprise DI Architecture** - Complete CRUD operations with dependency injection, enhanced security, and code intelligence capabilities!

## 🚀 Features

- **🔍 Codebase Exploration**: Browse projects, directories, and files with intuitive tools
- **📝 Full Write Operations**: Create, update, and delete files and directories
- **🛡️ Enterprise Security**: Multi-layer security validation with configurable permissions
- **💾 Automatic Backups**: Timestamped backups for all destructive operations
- **📁 Multi-Project Support**: Manage multiple codebases from a single service  
- **⚡ High Performance**: Compiled C# with MCPSharp for fast, reliable responses
- **🌐 Cross-Platform**: Robust path normalization for Windows, Linux, and macOS
- **🧩 Extensible DI Architecture**: Clean dependency injection for enterprise-grade extensibility
- **📋 Rich Metadata**: File size, MIME types, encoding detection, and content analysis
- **🔄 Battle-Tested Transport**: Uses MCPSharp framework for robust JSON-RPC communication
- **🔒 Safety First**: Explicit confirmations required for destructive operations
- **🧠 Code Intelligence**: Advanced C# code analysis and manipulation capabilities

## 🛠️ Available Tools

### File Operations
| Tool | Description | Parameters |
|------|-------------|------------|
| `local-get-project-file` | Read file content with full metadata | `projectName`, `path` |
| `local-create-project-file` | Create new files with content | `projectName`, `path`, `content`, `createDirectories?`, `overwrite?` |
| `local-update-project-file` | Update existing files with new content | `projectName`, `path`, `content`, `createBackup?`, `backupSuffix?` |
| `local-delete-project-file` | Delete files with safety confirmations | `projectName`, `path`, `createBackup?`, `confirmDeletion` |

### Directory Operations  
| Tool | Description | Parameters |
|------|-------------|------------|
| `local-list-project-directories` | List directories in a project | `projectName`, `path?`, `filter?` |
| `local-create-project-directory` | Create new directories | `projectName`, `path`, `createParents?` |
| `local-delete-project-directory` | Delete directories with confirmations | `projectName`, `path`, `recursive?`, `confirmDeletion` |

### Project Management
| Tool | Description | Parameters |
|------|-------------|------------|
| `local-list-projects` | Get all configured projects with metadata | None |
| `local-list-project-files` | List files in a project directory | `projectName`, `path?`, `filter?` |

### Code Intelligence (Coming Soon)
| Tool | Description | Parameters |
|------|-------------|------------|
| `local-sync-indexes` | Index and analyze C# code structure across projects | `projectName?`, `forceRefresh?` |
| `local-list-project-classes` | List all classes in a project with metadata | `projectName`, `namespace?`, `filter?` |
| `local-get-class-details` | Get detailed class information including members | `projectName`, `className`, `includeMembers?` |
| `local-list-class-methods` | List all methods in a specific class | `projectName`, `className`, `filter?` |
| `local-list-class-properties` | List all properties in a specific class | `projectName`, `className`, `filter?` |
| `local-insert-class-method` | Add a new method to an existing class | `projectName`, `className`, `methodCode`, `insertLocation?` |
| `local-update-class-method` | Update an existing method in a class | `projectName`, `className`, `methodName`, `newCode` |
| `local-delete-class-method` | Remove a method from a class | `projectName`, `className`, `methodName`, `confirmDeletion` |
| `local-insert-class-property` | Add a new property to an existing class | `projectName`, `className`, `propertyCode`, `insertLocation?` |
| `local-update-class-property` | Update an existing property in a class | `projectName`, `className`, `propertyName`, `newCode` |
| `local-delete-class-property` | Remove a property from a class | `projectName`, `className`, `propertyName`, `confirmDeletion` |

## 📋 Prerequisites

- **.NET 9.0** or later
- **Windows, Linux, or macOS** (cross-platform compatible)
- **MCP-compatible client** (Claude Desktop, Continue, etc.)

## ⚙️ Installation

### 1. Clone the Repository
```bash
git clone https://github.com/mmeents/DaemonsMCP.git
cd DaemonsMCP
```

### 2. Configure Your Projects and Security
Create or edit `DaemonsMCP/daemonsmcp.json`:

```json
{
  "version": "2.0",
  "daemon": {
    "name": "DaemonMCP",
    "version": "2.0.0"
  },
  "projects": [
    {
      "name": "DaemonsMCP",
      "description": "Your main project description",
      "path": "C:\\path\\to\\DaemonsMCP",
      "enabled": true
    },
    {
      "name": "YourProject", 
      "description": "Another project description",
      "path": "C:\\path\\to\\your\\project",
      "enabled": true
    }
  ],
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".js", ".py", ".md", ".txt", ".json", ".xml", ".yml", ".yaml"],
    "blockedExtensions": [".exe", ".dll", ".bat", ".cmd", ".sh"],
    "maxFileSize": "10MB",
    "maxFileWriteSize": "5MB",
    "writeProtectedPaths": [".git", ".vs", "bin", "obj", "node_modules"]
  }
}
```

### 3. Build the Service
```bash
dotnet build --configuration Release
```

### 4. Test the Service (Optional)
Use the included test client to verify functionality before connecting to Claude.

## 🔌 MCP Client Setup

### Claude Desktop Configuration

Add to your Claude Desktop `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "daemonsmcp": {
      "command": "C:\\path\\to\\DaemonsMCP\\DaemonsMCP\\bin\\Release\\net9.0-windows7.0\\DaemonsMCP.exe",
      "args": []
    }
  }
}
```

**Location of config file:**
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

### Restart Claude Desktop
After updating the configuration, fully restart Claude Desktop to load the new MCP server.

## 🎯 Usage Examples

Once connected to your MCP client, you can perform complete development workflows:

### Explore Available Projects
```
What projects do you have access to?
```
*Returns: JSON list of all configured projects with names, descriptions, and paths*

### Navigate Project Structure  
```
Show me the directories in the DaemonsMCP project
```
*Returns: Clean JSON array of directory names*

### Read Files
```
Show me the content of ProjectTools.cs in the DaemonsMCP project
```
*Returns: Full file content with metadata (size, encoding, MIME type)*

### Create New Files
```
Create a new utility class called StringHelper.cs with basic string extension methods
```
*Creates: New file with automatic directory creation if needed*

### Update Existing Files
```
Add a new method to the StringHelper.cs file for reversing strings
```
*Updates: File content with automatic backup creation*

### Delete Files Safely
```
Delete the test file I created earlier, but make sure to back it up first
```
*Deletes: File with explicit confirmation and automatic backup*

### Create Directory Structures
```
Create a new directory called "Utilities" in the project
```
*Creates: Directory with parent creation if needed*

### Search for Files
```
List all .cs files in the DaemonsMCP/DaemonsMCP directory
```
*Returns: Filtered list of C# source files*

### Code Intelligence Operations (Coming Soon)
```
Index all the C# code in the DaemonsMCP project and show me the classes
```
*Indexes: Analyzes code structure and returns class hierarchy*

```
Show me all the methods in the ProjectFileService class
```
*Returns: Detailed method signatures, parameters, and return types*

```
Add a new validation method to the SecurityService class
```
*Inserts: New method with proper formatting and placement*

## 🔧 Project Structure

```
DaemonsMCP/
├── DaemonsMCP/              # Main MCP service application
│   ├── Program.cs           # DI container and hosted service setup
│   └── daemonsmcp.json      # Project configuration
├── DaemonsMCP.Core/         # Core business logic and services
│   ├── Services/            # Business logic services
│   │   ├── ProjectService.cs        # Project management
│   │   ├── ProjectFileService.cs    # File operations
│   │   ├── ProjectFolderService.cs  # Directory operations
│   │   ├── SecurityService.cs       # Security validation
│   │   └── ValidationService.cs     # Input validation
│   ├── Models/              # Data models and DTOs
│   ├── Config/              # Configuration management
│   ├── DaemonsTools.cs      # MCP tool implementations
│   ├── DaemonsToolsBridge.cs # MCPSharp integration bridge
│   ├── DiServiceBridge.cs   # DI container bridge
│   └── DaemonsMcpHostedService.cs # Background service host
├── DaemonsTester/           # Test client for validation
└── README.md
```

## 🛡️ Security Features

### Read Operations
- **File Type Filtering**: Configurable whitelist of allowed file extensions
- **Path Validation**: Prevents directory traversal attacks  
- **Binary File Detection**: Safely handles non-text files
- **Project Sandboxing**: Access strictly limited to configured project directories

### Write Operations
- **Global Write Toggle**: `allowWrite` flag to enable/disable all write operations
- **Write-Protected Paths**: Configurable list of protected directories (`.git`, `bin`, `obj`, etc.)
- **File Size Limits**: Separate limits for read and write operations
- **Content Size Validation**: Pre-validation of content before writing
- **Critical File Protection**: Extra protection for project files (`.csproj`, `Program.cs`, etc.)
- **Path Safety Validation**: Prevents directory traversal and malicious path patterns

### Code Intelligence Security
- **AST-Only Analysis**: Code parsing without execution for safety
- **Syntax Validation**: Ensures inserted code is syntactically correct
- **Backup Before Modification**: Automatic backups for all code changes
- **Class Boundary Respect**: Modifications stay within class boundaries

### Safety Features
- **Automatic Backups**: Timestamped backups for all update and delete operations
- **Explicit Confirmations**: Required `confirmDeletion=true` for all destructive operations
- **Project Boundary Enforcement**: All operations validated within project directories
- **Cross-Platform Path Normalization**: Handles path differences across operating systems

## 🔮 Architecture & Technical Details

### Built on MCPSharp
DaemonsMCP leverages the [MCPSharp](https://github.com/afrise/MCPSharp) framework for:
- **Reliable Transport**: Proper duplex pipe communication vs basic console I/O
- **Automatic JSON Handling**: CamelCase conversion and proper serialization
- **Attribute-Based Tools**: Clean, declarative tool definitions
- **Error Handling**: Graceful exception management and JSON-RPC compliance

### V2 Enterprise Architecture
- **Dependency Injection**: Full Microsoft.Extensions.DependencyInjection integration
- **Hosted Services**: Proper .NET background service lifecycle management
- **Service Separation**: Clean separation of concerns across multiple services
- **Bridge Pattern**: Seamless integration between MCPSharp and DI container
- **Configurable Logging**: Production-ready logging with multiple providers
- **Scoped Operations**: Proper resource management and disposal

### Custom Enhancements
- **Complete CRUD Operations**: Full create, read, update, delete functionality
- **Enterprise Security Model**: Multi-layer validation with comprehensive safety checks
- **Configuration-Driven**: JSON-based configuration for projects and security settings
- **Cross-Platform Compatibility**: Robust path handling for Windows, Linux, and macOS
- **Advanced File Handling**: MIME type detection, encoding analysis, binary file support
- **Performance Optimized**: Efficient file system operations with proper filtering
- **Professional Error Handling**: Detailed error messages with proper exception types

## 🚧 Roadmap

### ✅ **V1 - Core Foundation (Complete)**
- **✅ Read-only file access** (Complete - Fully Working)
- **✅ Robust transport layer** (MCPSharp integration complete)
- **✅ Complete write operations** (Complete - create, update, delete files and directories)
- **✅ Enterprise security system** (Complete - multi-layer validation)
- **✅ Cross-platform support** (Complete - Windows, Linux, macOS)

### ✅ **V2 - Enterprise Architecture (Complete)**
- **✅ Dependency injection architecture** (Complete - Full DI container integration)
- **✅ Service-oriented design** (Complete - Clean service separation)
- **✅ Hosted service lifecycle** (Complete - Proper startup/shutdown)
- **✅ Enhanced error handling** (Complete - Production-ready logging)
- **✅ Improved configuration management** (Complete - Flexible config loading)

### 🚧 **V3 - Code Intelligence (In Progress)**
- **📋 Planned**: **SyncIndexs tool** - Index and analyze C# code structure across projects
- **📋 Planned**: **Class discovery and navigation** - Browse classes, methods, and properties
- **📋 Planned**: **Code insertion tools** - Add methods and properties to existing classes
- **📋 Planned**: **Code update tools** - Modify existing methods and properties
- **📋 Planned**: **Code deletion tools** - Remove methods and properties safely
- **📋 Planned**: **AST-based parsing** - Syntax tree analysis for accurate code manipulation
- **📋 Planned**: **IntelliSense-like features** - Type information and member discovery

### 🔮 **V4 - Advanced Features (Future)**
- **📋 Planned**: Full-text search across codebases
- **📋 Planned**: File watching and change notifications
- **📋 Planned**: Project templates and scaffolding
- **📋 Planned**: Code refactoring tools (rename, extract method, etc.)
- **📋 Planned**: Batch operations for multiple files
- **📋 Planned**: Integration with build systems (MSBuild, dotnet CLI)
- **📋 Planned**: Code metrics and analysis (complexity, coverage)

## 🤝 Contributing

Contributions are welcome! Areas where help is especially appreciated:

- **Code Intelligence Implementation** (AST parsing, C# analysis)
- **Additional tool implementations** (search, git operations, etc.)
- **Performance optimizations**
- **Documentation improvements** 
- **Integration examples** (VS Code, other MCP clients)
- **Advanced security features**
- **Testing and validation**

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Install MCPSharp dependencies: `dotnet restore`
4. Make your changes and test with the included test client
5. Test with Claude Desktop before submitting
6. Commit: `git commit -m 'Add amazing feature'`
7. Push: `git push origin feature/amazing-feature`
8. Open a Pull Request

## 🐛 Troubleshooting

### Common Issues

**Empty responses in Claude:**
- Ensure you've restarted Claude Desktop after config changes
- Verify the executable path in your configuration is correct
- Check that your projects are properly configured in `daemonsmcp.json`

**Write operations not working:**
- Ensure `allowWrite: true` in your security configuration
- Check that file extensions are in the `allowedExtensions` list
- Verify the target directory is not in `writeProtectedPaths`
- Confirm you have write permissions on the target directory

**File access errors:**
- Confirm project paths exist and are accessible
- Check that file extensions are allowed in security filters
- Verify path separators match your operating system
- Ensure file sizes are within the configured limits

**Build issues:**
- Ensure .NET 9.0 is installed
- Run `dotnet restore` to install dependencies including MCPSharp
- Check that all project references are resolved

**V2 Specific Issues:**
- Ensure all services are properly registered in the DI container
- Check that the configuration file is accessible during startup
- Verify that MCPSharp bridge is properly initialized

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built on the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) specification
- Powered by [MCPSharp](https://github.com/afrise/MCPSharp) for robust transport
- Inspired by the need to give LLMs better codebase understanding and modification capabilities
- Special thanks to the MCP community for feedback and testing
- Super Special thanks to Claude Desktop for being an amazing MCP client!

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/mmeents/DaemonsMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/mmeents/DaemonsMCP/discussions)
- **Documentation**: Check the wiki for detailed setup guides

---

**Ready to give your LLM complete development superpowers? Star ⭐ this repo and let's build the future of AI-assisted development together!**