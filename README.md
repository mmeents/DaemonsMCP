# DaemonsMCP

**Give LLMs eyes on your codebase** 👁️‍🗨️

DaemonsMCP is a robust C# MCP (Model Context Protocol) service that provides LLMs with secure, comprehensive access to explore and analyze local codebases. Built on MCPSharp for reliable transport and JSON-RPC communication, it gives your AI assistant the ability to see, navigate, and understand your project files just like a developer would.

✅ **Fully Working** - Tested and compatible with Claude Desktop and other MCP clients!

## 🚀 Features

- **🔍 Codebase Exploration**: Browse projects, directories, and files with intuitive tools
- **📁 Multi-Project Support**: Manage multiple codebases from a single service  
- **🛡️ Security First**: Built-in file type filtering and path validation
- **⚡ High Performance**: Compiled C# with MCPSharp for fast, reliable responses
- **🧩 Extensible**: Clean architecture ready for custom tools and capabilities
- **📋 Rich Metadata**: File size, MIME types, encoding detection, and content analysis
- **🔄 Battle-Tested Transport**: Uses MCPSharp framework for robust JSON-RPC communication

## 🛠️ Available Tools

| Tool | Description | Parameters |
|------|-------------|------------|
| `local-list-projects` | Get all configured projects with metadata | None |
| `local-list-project-directories` | List directories in a project | `projectName`, `path?`, `filter?` |
| `local-list-project-files` | List files in a project directory | `projectName`, `path?`, `filter?` |
| `local-get-project-file` | Read file content with full metadata | `projectName`, `path` |

## 📋 Prerequisites

- **.NET 9.0** or later
- **Windows** (primary support - cross-platform compatible)
- **MCP-compatible client** (Claude Desktop, Continue, etc.)

## ⚙️ Installation

### 1. Clone the Repository
```bash
git clone https://github.com/mmeents/DaemonsMCP.git
cd DaemonsMCP
```

### 2. Configure Your Projects
Create or edit `DaemonsMCP/daemonsmcp.json`:

```json
{
  "projects": [
    {
      "name": "DaemonsMCP",
      "description": "Your main project description",
      "path": "C:\\GithubMM\\DaemonsMCP"
    },
    {
      "name": "YourProject", 
      "description": "Another project description",
      "path": "C:\\path\\to\\your\\project"
    }
  ]
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

Once connected to your MCP client, you can:

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

### Examine Specific Files
```
Show me the content of ProjectTools.cs in the DaemonsMCP project
```
*Returns: Full file content with metadata (size, encoding, MIME type)*

### Search for Files
```
List all .cs files in the DaemonsMCP/DaemonsMCP directory
```
*Returns: Filtered list of C# source files*

## 🔧 Project Structure

```
DaemonsMCP/
├── DaemonsMCP/              # Main MCP service
│   ├── Program.cs           # MCPSharp service initialization
│   ├── ProjectTools.cs   # MCP tools with attributes
│   ├── ProjectHandler.cs    # Core file system operations
│   ├── GlobalConfig.cs      # Configuration management
│   ├── SecurityFilter.cs    # File access security
│   ├── MimeHelper.cs        # File type detection
│   └── daemonsmcp.json      # Project configuration
├── DaemonsTester/           # Test client for validation
└── README.md
```

## 🛡️ Security Features

- **File Type Filtering**: Configurable whitelist of allowed file extensions
- **Path Validation**: Prevents directory traversal attacks  
- **Binary File Detection**: Safely handles non-text files
- **Project Sandboxing**: Access strictly limited to configured project directories
- **Security-First Design**: All file operations go through security validation

## 🔮 Architecture & Technical Details

### Built on MCPSharp
DaemonsMCP leverages the [MCPSharp](https://github.com/afrise/MCPSharp) framework for:
- **Reliable Transport**: Proper duplex pipe communication vs basic console I/O
- **Automatic JSON Handling**: CamelCase conversion and proper serialization
- **Attribute-Based Tools**: Clean, declarative tool definitions
- **Error Handling**: Graceful exception management and JSON-RPC compliance

### Custom Enhancements
- **Manual JSON Serialization**: For complete control over response format
- **Project Configuration System**: Flexible JSON-based project management
- **Advanced File Handling**: MIME type detection, encoding analysis, binary file support
- **Performance Optimized**: Efficient file system operations with proper filtering

## 🚧 Roadmap

- **✅ Read-only file access** (Current - Fully Working)
- **✅ Robust transport layer** (MCPSharp integration complete)
- **🚧 Write operations** (create, update, delete files)
- **📋 Planned**: Full-text search across codebases
- **📋 Planned**: File watching and change notifications
- **📋 Planned**: Additonal tools (conversation whiteboard so these different conversation threads can chat with each other.)
- **📋 Planned**: Code intelligence (syntax highlighting, AST parsing)

## 🤝 Contributing

Contributions are welcome! Areas where help is especially appreciated:

- **Additional tool implementations** (search, git operations, etc.)
- **Cross-platform testing** (Linux/macOS validation)
- **Performance optimizations**
- **Documentation improvements** 
- **Integration examples** (VS Code, other MCP clients)

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

**File access errors:**
- Confirm project paths exist and are accessible
- Check that file extensions are allowed in security filters
- Verify path separators match your operating system

**Build issues:**
- Ensure .NET 8.0 is installed
- Run `dotnet restore` to install dependencies including MCPSharp
- Check that all project references are resolved

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built on the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) specification
- Powered by [MCPSharp](https://github.com/afrise/MCPSharp) for robust transport
- Inspired by the need to give LLMs better codebase understanding
- Special thanks to the MCP community for feedback and testing
- Super Special thanks to Claude Desktop for being an amazing MCP client!

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/mmeents/DaemonsMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/mmeents/DaemonsMCP/discussions)
- **Documentation**: Check the wiki for detailed setup guides

---

**Ready to give your LLM superpowers? Star ⭐ this repo and let's build the future of AI-assisted development together!**