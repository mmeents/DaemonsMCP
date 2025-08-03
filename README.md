# DaemonsMCP

**Give LLMs eyes on your codebase** 👁️‍🗨️

DaemonsMCP is a compiled C# MCP (Model Context Protocol) service that provides LLMs with comprehensive access to explore and analyze local codebases. Think of it as giving your AI assistant the ability to see, navigate, and understand your project files just like a developer would.

Please note: this is under construction and is changing.  Connecting via Claude isn't working. run ok when the tester runs it.  Checking it in to let Claude see it in it's current state.

## 🚀 Features

- **🔍 Codebase Exploration**: Browse projects, directories, and files with intuitive tools
- **📁 Multi-Project Support**: Manage multiple codebases from a single service
- **🛡️ Security First**: Built-in file type filtering and path validation
- **⚡ High Performance**: Compiled C# daemon for fast response times
- **🧩 Extensible**: Clean architecture ready for custom tools and capabilities
- **📋 Rich Metadata**: File size, MIME types, encoding detection, and content analysis

## 🛠️ Available Tools

| Tool | Description | Parameters |
|------|-------------|------------|
| `local/list-projects` | Get all available projects | None |
| `local/list-project-directories` | List directories in a project | `projectName`, `path?`, `filter?` |
| `local/list-project-files` | List files in a project directory | `projectName`, `path?`, `filter?` |
| `local/get-project-file` | Read file content with metadata | `projectName`, `path` |

## 📋 Prerequisites

- **.NET 8.0** or later
- **Windows** (Linux/macOS support coming soon)
- **MCP-compatible client** (Claude Desktop, Continue, etc.)

## ⚙️ Installation

### 1. Clone the Repository
```bash
git clone https://github.com/mmeents/DaemonsMCP.git
cd DaemonsMCP
```

### 2. Configure Your Projects
Edit `DaemonsMCP/Project.cs` to point to your local codebases:

```csharp
public static readonly string BasePath = "C:\\";        
public static readonly string Project1Path = Path.Combine(BasePath, "YourProject\\Path");

public static readonly Project project1 = new Project(
    "YourProjectName", 
    "Description of your project", 
    Project1Path);
```

### 3. Build the Service
```bash
dotnet build --configuration Release
```

### 4. Test the Service (Optional)
Use the included Windows Forms tester:
```bash
cd DaemonsTester
dotnet run
```

## 🔌 MCP Client Setup

### Claude Desktop Configuration

Add to your Claude Desktop `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "daemonsmcp": {
      "command": "C:\\path\\to\\DaemonsMCP\\bin\\Release\\net8.0\\DaemonsMCP.exe",
      "args": []
    }
  }
}
```

### Continue.dev Configuration

Add to your Continue config:

```json
{
  "mcpServers": [
    {
      "name": "daemonsmcp",
      "command": "C:\\path\\to\\DaemonsMCP\\bin\\Release\\net8.0\\DaemonsMCP.exe"
    }
  ]
}
```

## 🎯 Usage Examples

Once connected to your MCP client, you can:

### Explore Available Projects
```
Show me what projects are available
```

### Navigate Project Structure  
```
List the directories in the DaemonsMCP project
```

### Examine Specific Files
```
Show me the content of Program.cs in the DaemonsMCP project
```

### Search for Files
```
Find all .cs files in the DaemonsMCP project
```

## 🔧 Project Structure

```
DaemonsMCP/
├── DaemonsMCP/              # Main MCP service
│   ├── Program.cs           # Entry point & initialization
│   ├── ToolsHandler.cs      # MCP tool routing & validation
│   ├── ProjectHandler.cs    # Core file system operations
│   ├── Project.cs           # Project configuration
│   ├── SecurityFilter.cs    # File access security
│   ├── MimeHelper.cs        # File type detection
│   └── JsonRpcMessages.cs   # MCP protocol messages
├── DaemonsTester/           # Windows Forms test client
│   ├── Form1.cs            # GUI test interface
│   ├── McpClient.cs        # MCP client implementation
│   └── Program.cs          # Test app entry point
└── README.md
```

## 🛡️ Security Features

- **File Type Filtering**: Configurable whitelist of allowed file extensions
- **Path Validation**: Prevents directory traversal attacks
- **Binary File Detection**: Safely handles non-text files
- **Project Sandboxing**: Access limited to configured project directories

## 🔮 Roadmap

- **✅ Read-only file access** (Current)
- **🚧 Write operations** (create, update, delete files)
- **🚧 Cross-platform support** (Linux, macOS)
- **📋 Planned**: Full-text search across codebases
- **📋 Planned**: File watching and change notifications
- **📋 Planned**: Configuration file support (JSON/YAML)

## 🤝 Contributing

Contributions are welcome! Areas where help is especially appreciated:

- **Cross-platform support** (Linux/macOS compatibility)
- **Additional file format support**
- **Performance optimizations**
- **Documentation improvements**
- **Test coverage expansion**

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and test thoroughly
4. Commit: `git commit -m 'Add amazing feature'`
5. Push: `git push origin feature/amazing-feature`
6. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built on the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) specification
- Inspired by the need to give LLMs better codebase understanding
- Special thanks to the MCP community for feedback and testing

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/mmeents/DaemonsMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/mmeents/DaemonsMCP/discussions)
- **Documentation**: Check the wiki for detailed guides

---

**Ready to give your LLM superpowers? Star ⭐ this repo and let's build the future of AI-assisted development together!**