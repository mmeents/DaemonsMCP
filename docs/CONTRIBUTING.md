# Contributing to DaemonsMCP

Thank you for your interest in contributing to DaemonsMCP! This guide will help you get started.

## Quick Start for Contributors

1. **Fork the repository** on GitHub
2. **Clone your fork:**
   ```bash
   git clone https://github.com/yourusername/DaemonsMCP.git
   cd DaemonsMCP
   ```
3. **Create a feature branch:**
   ```bash
   git checkout -b feature/amazing-feature
   ```
4. **Make your changes** and test them
5. **Submit a pull request**

## Development Setup

### Prerequisites
- .NET 9.0 SDK
- Git
- Your favorite C# IDE (Visual Studio, VS Code, JetBrains Rider)

### Build and Test
```bash
# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Debug

# Run tests (when available)
dotnet test

# Test with Claude Desktop
dotnet run --configuration Debug
```

## Areas Needing Contribution

### High Priority
- **Performance optimizations** for large projects
- **Additional security features** and validation
- **More comprehensive testing** (unit and integration tests)
- **Bug fixes** for known issues

### Medium Priority  
- **Additional MCP tool implementations**
- **Integration examples** with other MCP clients
- **Documentation improvements** and examples
- **Cross-platform testing** and fixes

### Future Features
- **Git integration** (branch awareness, commit tracking)
- **Advanced code analysis** features
- **Build system integration** (MSBuild, dotnet CLI)
- **Enhanced project templates**

## Code Guidelines

### C# Conventions
- Follow standard C# naming conventions
- Use PascalCase for public members
- Use camelCase for private fields
- Add XML documentation for public APIs

### Project Structure
- Core business logic goes in `DaemonsMCP.Core`
- MCP tool implementations in `DaemonsTools.cs`
- Service interfaces and implementations in `Services/`
- Models and DTOs in `Models/`

### Security Considerations
- Always validate input parameters
- Respect security configuration settings
- Use the SecurityService for all security checks
- Never bypass path validation or file extension checks

## Pull Request Process

1. **Update documentation** if needed
2. **Add tests** for new functionality
3. **Ensure all tests pass**
4. **Update CHANGELOG.md** with your changes
5. **Submit PR** with clear description

### PR Description Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature  
- [ ] Documentation update
- [ ] Performance improvement

## Testing
- [ ] Tested with Claude Desktop
- [ ] Unit tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows project conventions
- [ ] Self-review completed
- [ ] Documentation updated
```

## Testing Guidelines

### Manual Testing
- Test with Claude Desktop integration
- Verify all tools work as expected
- Test security boundaries
- Check performance with large projects

### Automated Testing
- Write unit tests for new services
- Integration tests for MCP tools
- Performance tests for indexing operations

## Documentation Standards

### Code Documentation
```csharp
/// <summary>
/// Gets file content with security validation
/// </summary>
/// <param name="projectName">Name of the configured project</param>
/// <param name="path">File path relative to project root</param>
/// <returns>File content and metadata</returns>
public async Task<FileResponse> GetFileAsync(string projectName, string path)
```

### README Updates
- Keep examples current
- Update tool lists when adding new tools
- Maintain compatibility information

## Reporting Issues

### Bug Reports
Include:
- DaemonsMCP version
- Operating system
- .NET version
- Configuration file (sanitized)
- Steps to reproduce
- Expected vs actual behavior

### Feature Requests
Include:
- Use case description
- Proposed solution
- Alternative approaches considered
- Impact on existing functionality

## Getting Help

- **GitHub Discussions** for questions
- **GitHub Issues** for bugs and features
- **Code review** feedback on PRs

## Recognition

Contributors will be:
- Listed in CONTRIBUTORS.md
- Mentioned in release notes
- Credited in documentation

Thank you for helping make DaemonsMCP better! 🚀
