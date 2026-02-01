# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DaemonsMCP is a C# MCP (Model Context Protocol) service that gives LLMs secure read/write access to local codebases. It runs as a console app communicating via MCPSharp (JSON-RPC over stdio) and also has a REST API host with an Angular config UI.

**Version:** 3.0.0 | **Framework:** .NET 9.0 | **Database:** SQL Server via EF Core | **License:** MIT

## Build & Run Commands

```bash
# Build entire solution
dotnet build

# Run MCP server (console app, stdio transport)
dotnet run --project server/DaemonsMCP/Daemons3MCP.csproj

# Run REST API (web host with Swagger)
dotnet run --project server/DaemonsMCP.Api/

# Run web app (alternative host)
dotnet run --project Daemons.Web/

# Run all tests
dotnet test

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Add EF Core migration (run from repo root, startup project is the API)
dotnet ef migrations add MigrationName --project server/DaemonsMCP.Infrastructure --startup-project server/DaemonsMCP.Api

# Angular client
cd client/config-viewer && npm install && ng serve
```

## Architecture

Clean Architecture with four layers. Dependencies flow inward: Infrastructure/API -> Application -> Domain.

### Solution Projects (DaemonsMCP.sln)

- **Daemons3MCP** (`server/DaemonsMCP/`) - Console app entry point. Configures Serilog, builds host, registers `AddApplication()` + `AddInfrastructure()`.
- **DaemonsMCP.Domain** (`server/DaemonsMCP.Domain/`) - Entities, repository interfaces, constants (`Cx.cs`), enums, extensions. Zero external dependencies.
- **DaemonsMCP.Application** (`server/DaemonsMCP.Application/`) - CQRS handlers via MediatR. Organized by feature (FileSystem/, Projects/, Items/, Models/, ObjectHierarchy/, etc.). Auto-discovered from assembly.
- **DaemonsMCP.Infrastructure** (`server/DaemonsMCP.Infrastructure/`) - EF Core DbContext, repository implementations, background services, MCP tool handlers. Contains `DependencyInjection.cs` that wires everything.
- **DaemonsMCP.Api** (`server/DaemonsMCP.Api/`) - ASP.NET Core minimal API with Swagger. Maps endpoints via extension methods in `Extensions/`. Uses `AddWebInfra()` (no MCP server registration).
- **Daemons.Web** (`Daemons.Web/`) - Alternative ASP.NET Core web host.
- **DaemonsMCP.xUnit.Tests** (`DaemonsMCP.xUnit.Tests/`) - xUnit tests covering Git, Scriban templates, token extensions.

### MCP Tool Pattern

MCP tools follow a two-class pattern in `Infrastructure/Tools/`:

1. **Tools class** (e.g., `ProjectTools.cs`) - Static methods with `[McpTool]` attributes. Resolves handler via `DIServiceBridge.GetService<T>()` (a static service locator bridging MCPSharp's requirement for parameterless constructors with DI).
2. **Handler class** (e.g., `ProjectToolsHandler.cs`) - Registered as singleton. Creates a DI scope, resolves `IMediator`, sends queries/commands. Returns `McpOpResult` serialized as JSON.

Tools are registered in `McpServerHostedService.ExecuteAsync()` via `MCPServer.Register<T>()`.

### CQRS Pattern

Each feature folder in `Application/` contains Query and Command classes with their MediatR handlers:
- Queries for reads: e.g., `GetAllProjectsQuery` -> `GetAllProjectsQueryHandler`
- Commands for writes: e.g., `CreateProjectFileCommand` -> `CreateProjectFileCommandHandler`
- Handlers use repository interfaces, never DbContext directly.

### Key Infrastructure Services

- **McpServerHostedService** - Starts MCPSharp stdio server, registers all tool classes
- **FileWatcherHostedService** - Monitors filesystem changes, triggers reindexing
- **IndexingService** - Parses C# code via Roslyn into ObjectHierarchy (namespaces, classes, methods)
- **FileSystemSyncService** - Syncs physical files with FileSystemNode database records
- **ValidationService** - Input validation and security checks on all handlers

### Data Model

Core entities: `Project`, `FileSystemNode` (file/dir tree), `Item`/`ItemType` (hierarchical nodes for todos/notes), `ObjectHierarchy`/`Identifier` (code index), `Model`/`ModelType`/`ModelProperty` (template system), `GitRepository`/`GitBranch`, `AccessToken`/`User`/`InvitationToken`.

## Configuration

Config file is `daemonsmcp.json` (not `appsettings.json`). Key setting:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DaemonsMCP;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

App data stored at `%ProgramData%/Daemons3MCP/` (logs, keys subdirectories) via `CommonPath`.

## Important Constants

All MCP tool names and descriptions are defined in `DaemonsMCP.Domain/Constants/Cx.cs`. Well-known IDs:
- ItemTypes: None=1, Categories=2, Todo=5, Readme=6, Note=7
- StatusTypes: NotStarted=10, InProgress=11, Complete=12, OnHold=13, Cancelled=14

## DI Registration

- `AddApplication()` in `DaemonsMCP.Application/DependencyInjection.cs` - registers MediatR handlers
- `AddInfrastructure()` in `DaemonsMCP.Infrastructure/DependencyInjection.cs` - registers DbContext, repositories (scoped), services (scoped), tool handlers (singleton), hosted services
- `AddWebInfra()` - same as `AddInfrastructure` but without MCP tool handlers and McpServerHostedService (for web hosts)

## Conventions

- MCP tool names: kebab-case (defined as constants in `Cx.cs`)
- Repository interfaces in Domain, implementations in Infrastructure
- All tool handler methods create a DI scope before resolving IMediator
- The main MCP server project targets `net9.0-windows7.0`; API and Web projects target `net9.0`
- API uses minimal API pattern with endpoint extension methods (e.g., `app.MapProjectEndpoints()`)
