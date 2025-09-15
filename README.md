# DaemonsMCP

**Give LLMs hands-on control of your codebase** 🐁️‍🗨️✋

DaemonsMCP is a comprehensive C# MCP (Model Context Protocol) service that provides LLMs with secure, full-featured access to explore, read, and **write** to local codebases. Built on MCPSharp for reliable transport and JSON-RPC communication, it gives your AI assistant the ability to see, navigate, understand, and **modify** your project files just like a developer would.

✅ **V2 - Enterprise DI Architecture** - Complete CRUD operations with dependency injection, enhanced security, code intelligence, and project management capabilities!

## 🚀 Key Features

- **🔍 Codebase Exploration**: Browse projects, directories, and files with intuitive tools
- **📝 Full Write Operations**: Create, update, and delete files and directories
- **🛡️ Enterprise Security**: Multi-layer security validation with configurable permissions
- **💾 Automatic Backups**: Timestamped backups for all destructive operations
- **📁 Multi-Project Support**: Manage multiple codebases from a single service
- **⚡ High Performance**: Compiled C# with MCPSharp for fast, reliable responses
- **🌐 Cross-Platform**: Robust path normalization for Windows, Linux, and macOS
- **🧠 Code Intelligence**: Advanced C# code analysis and manipulation with Microsoft.CodeAnalysis
- **📊 Project Management**: Hierarchical nodes, todo lists, and project organization
- **🗃️ File-Based Database**: PackedTables.NET for efficient local storage

## 📚 Documentation

| Document | Description |
|----------|-------------|
| **[Quick Start Guide](docs/QUICKSTART.md)** | Get up and running in 5 minutes |
| **[Installation Guide](docs/INSTALLATION.md)** | Detailed setup instructions |
| **[Tool Reference](docs/TOOLS.md)** | Complete tool documentation |
| **[Configuration Guide](docs/CONFIGURATION.md)** | Security and project setup |
| **[Architecture Overview](docs/ARCHITECTURE.md)** | Technical implementation details |
| **[Troubleshooting](docs/TROUBLESHOOTING.md)** | Common issues and solutions |

## ⚡ Quick Start

1. **Clone and build:**
   ```bash
   git clone https://github.com/mmeents/DaemonsMCP.git
   cd DaemonsMCP
   dotnet build --configuration Release
   ```

2. **Configure projects** in `DaemonsMCP/daemonsmcp.json`

3. **Add to Claude Desktop config:**
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

4. **Restart Claude Desktop** and start coding!

## 🛠️ Available Tools (V2)

### File & Directory Operations
- `daemonsmcp:list-projects` - List all configured projects
- `daemonsmcp:list-project-files` - Browse project files
- `daemonsmcp:list-project-directories` - Browse project directories
- `daemonsmcp:get-project-file` - Read file content with metadata
- `daemonsmcp:create-project-file` - Create new files
- `daemonsmcp:update-project-file` - Update existing files
- `daemonsmcp:delete-project-file` - Delete files safely
- `daemonsmcp:create-project-directory` - Create directories
- `daemonsmcp:delete-project-directory` - Delete directories

### Code Intelligence
- `daemonsmcp:resync-index` - Manual ReSync the Index C# code structure
- `daemonsmcp:status-index` - Check indexing status
- `daemonsmcp:list-classes` - Browse classes with pagination
- `daemonsmcp:get-class` - Get class details and content
- `daemonsmcp:list-class-methods` - Browse class methods
- `daemonsmcp:get-class-method` - Get method implementation
- `daemonsmcp:add-update-class` - Add or modify classes
- `daemonsmcp:add-update-method` - Add or modify methods

### Project Management
Nodes functions are recursive and support hierarchical structures.
- `daemonsmcp:list-nodes` - Browse hierarchical project structure
- `daemonsmcp:get-nodes-by-id` - Get specific nodes
- `daemonsmcp:add-update-nodes` - Create/update project nodes
- `daemonsmcp:remove-node` - Delete nodes with cascade options
- `daemonsmcp:list-item-types` - Manage node types
- `daemonsmcp:list-status-types` - Manage node statuses

### Todo Management
- `daemonsmcp:make-todo-list` - Create organized todo lists (these are also available via nodes methods)
- `daemonsmcp:get-next-todo` - Get next task and mark in progress
- `daemonsmcp:mark-todo-done` - Complete tasks
- `daemonsmcp:mark-todo-cancel` - Cancel tasks
- `daemonsmcp:restore-todo` - Reset task status

## 📋 Prerequisites

- **.NET 9.0** or later
- **Windows, Linux, or macOS** (cross-platform compatible)
- **MCP-compatible client** (Claude Desktop, Continue, etc.)

## 🔧 Architecture

DaemonsMCP V2 features a modern enterprise architecture:

- **MCPSharp Foundation**: Reliable JSON-RPC transport
- **Dependency Injection**: Full Microsoft.Extensions.DependencyInjection
- **PackedTables.NET**: Efficient file-based database storage
- **Microsoft.CodeAnalysis**: Advanced C# parsing and manipulation
- **Hosted Services**: Proper .NET lifecycle management
- **Multi-layer Security**: Comprehensive validation and safety

## 🛡️ Security

DaemonsMCP includes enterprise-grade security features:

- **Project Sandboxing**: Access limited to configured directories
- **File Type Filtering**: Whitelist/blacklist file extensions
- **Write Protection**: Configurable protected paths
- **Size Limits**: Separate read/write size restrictions
- **Automatic Backups**: All destructive operations create backups
- **Explicit Confirmations**: Required for dangerous operations

## 🤝 Contributing

Contributions welcome! See our [contributing guidelines](docs/CONTRIBUTING.md) for details.

Areas of interest:
- Performance optimizations
- Additional MCP tool implementations
- Security enhancements
- Documentation improvements
- Integration examples

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built on the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) specification
- Powered by [MCPSharp](https://github.com/afrise/MCPSharp) for robust transport
- Special thanks to the MCP community and Claude Desktop!

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/mmeents/DaemonsMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/mmeents/DaemonsMCP/discussions)

---

**Ready to give your LLM complete development superpowers? Star ⭐ this repo and let's build the future of AI-assisted development together!**
