# DaemonsMCP Quick Start Guide

Get DaemonsMCP V2 up and running with Claude Desktop in 5 minutes!

## Prerequisites Check

✅ **.NET 9.0 SDK** installed  
✅ **Claude Desktop** installed  
✅ **Git** for cloning (or download ZIP)

## Step 1: Download & Build (2 minutes)

```bash
# Clone the repository
git clone https://github.com/mmeents/DaemonsMCP.git
cd DaemonsMCP

# Build the release version
dotnet build --configuration Release
```

**Build Output Location:**
- Windows: `DaemonsMCP\bin\Release\net9.0-windows7.0\DaemonsMCP.exe`
- Linux/macOS: `DaemonsMCP/bin/Release/net9.0/DaemonsMCP`

## Step 2: Configure Projects (2 minutes)

Edit `DaemonsMCP/daemonsmcp.json`:

```json
{
  "version": "2.0",
  "daemon": {
    "name": "DaemonMCP",
    "version": "2.0.0"
    "nodesFilePath": "C:\\MCPSandbox"
  },
  "projects": [
    {
      "name": "DaemonsMCP",
      "description": "The DaemonsMCP project itself",
      "path": "C:\\path\\to\\DaemonsMCP",
      "enabled": true
    },
    {
      "name": "MyProject", 
      "description": "Your project description",
      "path": "C:\\path\\to\\your\\project",
      "enabled": true
    }
  ],
  "security": {
    "allowWrite": true,
    "allowedExtensions": [".cs", ".js", ".py", ".md", ".txt", ".json", ".xml", ".yml", ".yaml"],
    "blockedExtensions": [".exe", ".dll", ".bat", ".cmd", ".sh"],
    "maxFileSize": "50KB",
    "maxFileWriteSize": "5MB",
    "writeProtectedPaths": [".git", ".vs", "bin", "obj", "node_modules"]
  }
}
```

**Key Points:**
- Use "nodesFilePath" to set the folder to where project management nodes are stored. empty or missing defaults to the `DaemonsMCP` app folder.
- Logging is enabled by default and logs to `DaemonsMCP/logs` folder. found sending logs back through standard error confuses clients. Clients also maintain logs in their respective folders.
- Use **forward slashes** or **escaped backslashes** in paths
- Set `allowWrite: true` to enable file creation/modification
- Add your actual project paths
- Customize allowed file extensions for your needs
- maxFileSize limits large files rom being read. It's set to 50KB by default to avoid huge files.  Larger files start having issues with filling up context too fast.  Can cause files to skip being indexed.

## Step 3: Configure Claude Desktop (1 minute)

### Find Your Config File

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

### Add DaemonsMCP Server

Edit or create the config file:

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

**For Linux/macOS:**
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

**⚠️ Important:** Use the **full absolute path** to the executable!

## Step 4: Test Connection

1. **Restart Claude Desktop completely** (close all windows, restart app)
2. **Open a new conversation** in Claude Desktop
3. **Test with a simple command:**

```
What projects do you have access to?
```

**Expected Response:**
Claude should return a JSON list of your configured projects showing names, descriptions, and paths.

## Step 5: Verify Full Functionality

### Test File Reading
```
Show me the README.md file from the DaemonsMCP project
```

### Test Directory Browsing  
```
List the directories in the DaemonsMCP project
```

### Test Code Intelligence
```
Index the C# code in the DaemonsMCP project and show me the first 10 classes
```

### Test Project Management
```
Show me the project documentation nodes with 2 levels of depth
```

## Common Quick Start Issues

### ❌ "No tools available" or Empty Responses

**Solutions:**
1. **Verify executable path** - Make sure the full path in claude_desktop_config.json is correct
2. **Restart Claude Desktop** - Close all windows and restart the application  
3. **Check .NET version** - Ensure .NET 9.0 is installed
4. **Test build** - Run `dotnet run` in the DaemonsMCP directory to verify it starts

### ❌ "Project not found" Errors

**Solutions:**
1. **Check project paths** - Ensure paths in daemonsmcp.json exist and are accessible
2. **Use absolute paths** - Relative paths may not work correctly
3. **Check permissions** - Ensure the user has read access to project directories

### ❌ Write Operations Failing

**Solutions:**
1. **Enable writes** - Set `"allowWrite": true` in security configuration
2. **Check file extensions** - Ensure target file extension is in `allowedExtensions`
3. **Verify paths** - Ensure target directory is not in `writeProtectedPaths`
4. **Check permissions** - Ensure write access to target directories

### ❌ Empty Results from list-classes or list-class-methods

**Solutions:**
1. **Specify pagination explicitly:**
   ```
   List classes in DaemonsMCP project, show 20 results on page 1
   ```
2. **Index the code first:**
   ```
   Index the C# code in all projects to make sure everything is analyzed
   ```
3. **Check indexing status:**
   ```
   What's the current status of the code indexing service?
   ```

## Example Workflows

### Basic File Operations
```
0. "Read the Readme file to get context on the DaemonsMCP project"
1. "What projects do you have access to?"
2. "List all .cs files in the DaemonsMCP/DaemonsMCP.Core directory"
3. "Show me the ProjectService.cs file"
4. "Create a new utility class called DateHelper.cs in the Utilities folder"
```

### Code Intelligence Workflow
```
1. "Index all C# code in the DaemonsMCP project"
2. "Use DaemonsMCP toos to find and review Anyfile."
3. "find bug in the ProjectService class "
4. "make a todo list to review and then add entry foreach methods in the Project namespace"
5. "Add a new validation method to the ProjectService class"
```

### Project Management Workflow
```
1. "Show me the project documentation structure"
2. "Create a todo list called 'Bug Fixes' with items for null reference handling and error logging"
3. "Get the next task from the Bug Fixes todo list"
4. "Mark todo item 15 as completed"
5. "Get next todo item, do it and mark as completed or Restore if unable to complete."
```

## Next Steps

✅ **Working?** Great! Check out the [Tool Reference](TOOLS.md) for complete capabilities  
✅ **Need more security?** See the [Configuration Guide](CONFIGURATION.md)  
✅ **Want to understand the architecture?** Read the [Architecture Overview](ARCHITECTURE.md)  
✅ **Having issues?** Check the [Troubleshooting Guide](TROUBLESHOOTING.md)

## Pro Tips

1. **Start Conversations** with read the Readme method to quickly get context up to speed on important usage details. Have a conversation context write important findings for the next. Readme are just nodes with type `Readme`
2. **Always specify pagination** for list operations to avoid empty results
3. **Use descriptive project names** in your configuration for easy reference
4. **Start with read-only operations** before enabling writes
5. **Index your code first** before using code intelligence features
6. **Use todo lists** to organize development tasks within the MCP interface

**Congratulations! You now have a powerful AI assistant with full access to your codebase! 🎉**
