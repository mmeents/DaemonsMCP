# DaemonsMCP Config Viewer - Angular Client

This is a local-only Angular application for viewing and managing DaemonsMCP configurations.

## Prerequisites
- Node.js v20.19+ or v22+
- npm 10+
- Angular CLI 20+

## Getting Started

### 1. Install Dependencies
```bash
cd client/config-viewer
npm install
```

### 2. Start the Backend API
Make sure your DaemonsMCP API is running in Visual Studio:
- Open the server solution in Visual Studio
- Run the `DaemonsMCP.Api` project
- Verify it's running at `https://localhost:44356`

### 3. Configure CORS (Backend)
If you haven't already, you need to enable CORS in your API to allow requests from the Angular app running on `http://localhost:4200`.

In your `Program.cs` or `Startup.cs`, add:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// After building the app
app.UseCors("LocalDev");
```

### 4. Start the Angular Dev Server
```bash
ng serve
```

The app will be available at `http://localhost:4200`

## Project Structure

```
client/config-viewer/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   └── services/          # API services
│   │   │       ├── api.service.ts
│   │   │       ├── projects.service.ts
│   │   │       ├── filesystem.service.ts
│   │   │       ├── hierarchy.service.ts
│   │   │       └── indexing.service.ts
│   │   ├── shared/
│   │   │   └── models/            # TypeScript interfaces
│   │   │       └── api.models.ts
│   │   ├── features/              # Feature modules (to be added)
│   │   ├── app.ts                 # Main app component
│   │   ├── app.html               # App template
│   │   ├── app.scss               # App styles
│   │   └── app.config.ts          # App configuration
│   └── environments/
│       ├── environment.ts         # Dev environment config
│       └── environment.production.ts
```

## Available Services

### ProjectsService
- `getProjects()` - List all projects
- `getProject(id)` - Get project by ID
- `createProject(command)` - Create a new project
- `getReadme()` - Get readme content
- `checkHealth()` - Health check

### FileSystemService
- `searchFileSystem(params)` - Search files and directories
- `getFileSystemNode(projectId, nodeId)` - Get file/folder details
- `createFile(projectId, path, content)` - Create a file
- `createFolder(projectId, path)` - Create a folder
- `updateFile(projectId, nodeId, content)` - Update a file
- `syncFileSystem(projectId)` - Sync filesystem

### HierarchyService
- `searchObjectHierarchy(params)` - Search namespaces, classes, methods

### IndexingService
- `runIndexing(projectId)` - Trigger indexing
- `getQueueStatus(projectId)` - Get indexing queue status

## Next Steps

1. **Test the Connection**: The current app displays all projects from your API
2. **Add File Browser**: Create a component to browse the file system
3. **Add Code Viewer**: Create a component to view and search classes/methods
4. **Add Indexing Controls**: Create a component to manage indexing

## Development Notes

- This is a **local-only** application (no authentication required)
- All API calls are made to `https://localhost:44356`
- The app uses Angular standalone components (Angular 17+ style)
- TypeScript models match your OpenAPI spec

## Common Issues

### CORS Error
If you see CORS errors in the browser console:
1. Make sure CORS is configured in your backend API
2. Verify the backend is running at `https://localhost:44356`
3. Check that the Angular app is running on `http://localhost:4200`

### SSL Certificate Error
If you see SSL certificate errors:
1. Your backend is using HTTPS with a self-signed certificate
2. Navigate to `https://localhost:44356/health` in your browser
3. Accept the certificate warning
4. Return to the Angular app and refresh
