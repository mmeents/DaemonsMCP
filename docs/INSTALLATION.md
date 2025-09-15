# DaemonsMCP Installation Guide

Complete installation guide for DaemonsMCP V2 across different platforms and MCP clients.

## System Requirements

### Minimum Requirements
- **.NET 9.0 SDK or Runtime**
- **2GB RAM** (for indexing large projects)
- **100MB disk space** (plus space for index files)
- **Windows 10/11, Linux, or macOS 10.15+**

### Recommended Requirements  
- **.NET 9.0 SDK** (for development)
- **4GB RAM** (for multiple large projects)
- **500MB disk space** (with room for backups)
- **SSD storage** (for faster indexing)

## Prerequisites Installation

### Windows

**Install .NET 9.0:**
1. Download from [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Run the installer (choose SDK for development, Runtime for deployment)
3. Verify installation:
   ```cmd
   dotnet --version
   ```
   Should show `9.0.x` or later

### macOS

**Using Homebrew (recommended):**
```bash
# Install .NET 9.0
brew install --cask dotnet

# Verify installation
dotnet --version
```

### Linux (Ubuntu/Debian)

**Using Microsoft Package Repository:**
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Update and install
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Verify installation
dotnet --version
```

## DaemonsMCP Installation

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/mmeents/DaemonsMCP.git
cd DaemonsMCP

# Restore dependencies
dotnet restore

# Build release version
dotnet build --configuration Release
```

**Build Output Locations:**
- **Windows**: `DaemonsMCP\bin\Release\net9.0-windows7.0\DaemonsMCP.exe`
- **Linux/macOS**: `DaemonsMCP/bin/Release/net9.0/DaemonsMCP`

### Configuration Setup

Create `DaemonsMCP/DaemonsMCP/daemonsmcp.json`:

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
      "description": "The DaemonsMCP project itself",
      "path": "/absolute/path/to/DaemonsMCP",
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

## Claude Desktop Setup

### Find Configuration File

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

### Configure MCP Server

Edit or create the configuration file:

**Windows:**
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

**macOS/Linux:**
```json
{
  "mcpServers": {
    "daemonsmcp": {
      "command": "/path/to/DaemonsMCP/DaemonsMCP/bin/Release/net9.0/DaemonsMCP",
      "args": []
    }
  }
}
```

### Restart Claude Desktop

1. Close all Claude Desktop windows
2. Quit Claude Desktop completely
3. Restart Claude Desktop
4. Open a new conversation

## Testing Installation

### Basic Test
```
What projects do you have access to?
```

### File Operations Test
```
List the files in the DaemonsMCP project root directory
```

### Code Intelligence Test
```
Index the C# code in the DaemonsMCP project
```

## Common Issues

### Empty Responses
- Verify executable path in claude_desktop_config.json
- Restart Claude Desktop completely
- Check .NET 9.0 is installed

### Project Not Found
- Use absolute paths in daemonsmcp.json
- Verify project directories exist
- Check file permissions

### Write Operations Failing
- Set `"allowWrite": true` in security config
- Check file extensions are allowed
- Verify target paths not in writeProtectedPaths

## Deployment Options

### Development Environment
```json
{
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".js", ".py", ".md", ".txt", ".json"],
    "maxFileWriteSize": "10MB"
  },
  "logging": {
    "level": "Debug"
  }
}
```

### Production Environment
```json
{
  "security": {
    "allowWrite": false,
    "allowedExtensions": [".md", ".txt"],
    "maxFileSize": "5MB"
  },
  "logging": {
    "level": "Error"
  }
}
```

## Performance Optimization

### Large Projects
- Use SSD storage for index files
- Increase memory limits if needed
- Consider disabling file watching for very large projects

### Memory Settings
```bash
export DOTNET_GCHeapHardLimit=2147483648  # 2GB limit
dotnet run --configuration Release
```

## Troubleshooting

### Check .NET Installation
```bash
dotnet --info
```

### Validate Configuration
```bash
dotnet run --configuration Release -- --validate-config
```

### Test Executable
```bash
./DaemonsMCP/bin/Release/net9.0/DaemonsMCP --version
```

### Enable Debug Logging
```json
"logging": {
  "level": "Debug",
  "enableConsole": true,
  "enableFile": true,
  "filePath": "debug.log"
}
```

## Next Steps

- [Quick Start Guide](QUICKSTART.md) - Get running in 5 minutes
- [Configuration Guide](CONFIGURATION.md) - Detailed configuration options
- [Tool Reference](TOOLS.md) - Complete API documentation
- [Troubleshooting](TROUBLESHOOTING.md) - Common issues and solutions

## Support

- **GitHub Issues**: [Report problems](https://github.com/mmeents/DaemonsMCP/issues)
- **Discussions**: [Ask questions](https://github.com/mmeents/DaemonsMCP/discussions)
