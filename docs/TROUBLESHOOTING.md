# DaemonsMCP Troubleshooting Guide

Common issues, solutions, and debugging strategies for DaemonsMCP V2.

## Quick Diagnosis Checklist

### ❌ No tools available in Claude Desktop
1. ✅ Check executable path in `claude_desktop_config.json`
2. ✅ Restart Claude Desktop completely 
3. ✅ Verify .NET 9.0 is installed: `dotnet --version`
4. ✅ Test executable directly: `./DaemonsMCP.exe --version`

### ❌ Empty responses from list-classes/list-class-methods
1. ✅ Specify explicit pagination: `pageNo=1, itemsPerPage=20`
2. ✅ Index code first: Run `resync-index`
3. ✅ Check indexing status: Use `status-index`

### ❌ Write operations failing
1. ✅ Enable writes: `"allowWrite": true` in config
2. ✅ Check file extensions: Ensure extension in `allowedExtensions`
3. ✅ Verify paths: Ensure not in `writeProtectedPaths`
4. ✅ Check permissions: Verify write access to directories

## Claude Desktop Issues

### Configuration Problems
**Issue: Tools not appearing in Claude Desktop**

**Solutions:**
1. **Check config file location:**
   - Windows: `%APPDATA%\Claude\claude_desktop_config.json`
   - macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`

2. **Use absolute paths:**
   ```json
   {
     "mcpServers": {
       "daemonsmcp": {
         "command": "C:\\full\\path\\to\\DaemonsMCP.exe",
         "args": []
       }
     }
   }
   ```

3. **Restart Claude Desktop properly:**
   - Close ALL windows
   - Quit from system tray/menu bar
   - Wait 5 seconds
   - Restart and open new conversation

### Testing Connection
```
What projects do you have access to?
```
Should return JSON list of your configured projects.

## Installation Issues

### .NET Problems
**Issue: "dotnet command not found"**
```bash
# Install .NET 9.0 SDK
# Windows: Download from microsoft.com/dotnet
# macOS: brew install --cask dotnet
# Linux: Follow Microsoft package repository guide

# Verify
dotnet --version  # Should show 9.0.x
```

### Build Problems
**Issue: Build fails**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build --configuration Release
```

### Permission Problems
**Issue: Executable won't run**
```bash
# Linux/macOS: Make executable
chmod +x DaemonsMCP/bin/Release/net9.0/DaemonsMCP

# Windows: Check antivirus, run as admin if needed
```

## Configuration Issues

### Path Problems
**❌ Common mistakes:**
```json
{
  "path": "..\\MyProject",     // Relative paths
  "path": "C:MyProject"        // Missing separators
}
```

**✅ Correct format:**
```json
{
  "path": "C:/projects/MyProject",          // Forward slashes
  "path": "/home/user/projects/MyProject"   // Unix paths
}
```

### Security Issues
**Issue: Files not accessible**
- Check file extension is in `allowedExtensions`
- Ensure `allowWrite: true` for write operations
- Verify target path not in `writeProtectedPaths`

## Code Intelligence Issues

### Critical Pagination Bug
**Issue: list-classes returns empty results**

**Solution:** Always specify explicit pagination
```
# ❌ This often fails:
List classes in DaemonsMCP project

# ✅ This works:
List classes in DaemonsMCP project, show 20 results on page 1
```

### Indexing Problems
**Issue: No classes found after indexing**
1. Check indexing status: `What's the indexing service status?`
2. Force rebuild: `Index all C# code with force rebuild`
3. Verify .cs files exist in project

### Critical Casing Bug
**Issue: add-update-method fails with serialization error**

**Solution:** Use PascalCase properties
```json
// ✅ Correct
{
  "ClassId": 1,
  "MethodName": "MyMethod",
  "Content": "method code"
}

// ❌ Wrong - causes serialization errors
{
  "classId": 1,
  "methodName": "MyMethod", 
  "content": "method code"
}
```

## File Operation Issues

### Permission Errors
**Issue: "Access denied" reading/writing files**
1. Check file/directory permissions
2. Verify DaemonsMCP process has access
3. Check antivirus isn't blocking operations

### Path Resolution
**Issue: "File not found" with correct path**
1. Verify project path in config is correct
2. Use paths relative to project root
3. Check case sensitivity on Linux/macOS

## Performance Issues

### Slow Operations
- Use SSD storage for better performance
- Reduce batch sizes for large projects
- Check available memory and disk space

### Memory Usage
```bash
# Limit .NET memory if needed
export DOTNET_GCHeapHardLimit=1073741824  # 1GB
dotnet run --configuration Release
```

## Debugging Steps

### Enable Debug Logging
```json
{
  "logging": {
    "level": "Debug",
    "enableConsole": true,
    "enableFile": true,
    "filePath": "debug.log"
  }
}
```

### Test Components
```bash
# Test .NET installation
dotnet --info

# Test executable
./DaemonsMCP.exe --version

# Validate configuration  
dotnet run -- --validate-config
```

### Common Error Patterns
- `Project not found`: Check project paths in config
- `Extension not allowed`: Add to allowedExtensions
- `Path is write protected`: Remove from writeProtectedPaths
- `Serialization error`: Check PascalCase in object properties

## Getting Help

1. **Check logs** with debug level enabled
2. **Test with minimal config** to isolate issues
3. **Verify each component** works independently
4. **Create GitHub issue** with logs and config (sanitized)

For complex issues, provide:
- Configuration file (remove sensitive paths)
- Error messages or unexpected behavior
- Debug logs
- Operating system and .NET version
