using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Extensions;
using OperationResult = DaemonsMCP.Core.Models.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Core.Services {
  public class ProjectFileService(
      IAppConfig config,
      ILoggerFactory loggerFactory,
      IValidationService validationService,
      ISecurityService securityService) : IProjectFileService {
    private readonly IAppConfig _config = config;
    private readonly ILogger<ProjectFileService> _logger = loggerFactory.CreateLogger<ProjectFileService>();
    private readonly IValidationService _validationService = validationService;
    private readonly ISecurityService _securityService = securityService;

    public Task<IEnumerable<string>> GetFilesAsync(string projectName, string? path = null, string? filter = null) {
      
      var context = _validationService.ValidateAndPrepare(projectName, path ?? "", true, true);
      var fullPath = context.FullPath+ Path.DirectorySeparatorChar;
            
      var searchPattern = string.IsNullOrEmpty(filter) ? "*" : filter;

      var files = Directory.GetFiles(fullPath, searchPattern, SearchOption.TopDirectoryOnly)
        .Where(file => _securityService.IsFileAllowed(file))
        .Select(file => file[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar));

      return Task.FromResult(files);      

    }

    public async Task<FileContent> GetFileAsync(string projectName, string path) {

      _validationService.ValidatePath(path);

      var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false, false );
      var fullPath = context.FullPath;
         

      var fileInfo = new FileInfo(fullPath);
      var relativePath = fileInfo.FullName[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);
      var fileExtension = fileInfo.Extension.ToLowerInvariant();
      var fileEncoding = MimeTypesMap.DetectFileEncoding(fullPath);
      string contentType = MimeTypesMap.GetMimeType(fileExtension);
      bool isBinary = MimeTypesMap.IsBinaryFile(fullPath);

      var fileContent = "";
      if (!isBinary) {
        fileContent = await File.ReadAllTextAsync(fullPath, System.Text.Encoding.UTF8).ConfigureAwait(false);
      }

      return new FileContent(){
        FileName = fileInfo.Name,
        Path = relativePath,
        Size = fileInfo.Length,
        ContentType = contentType,
        Encoding = fileEncoding,
        Content = fileContent,
        IsBinary = isBinary
      };
    }


    public async Task<OperationResult> CreateFileAsync(string projectName,  string path, string content) {
      try {
        _validationService.ValidatePath(path);
        _validationService.ValidateContent(content);      
        var context = _validationService.ValidateAndPrepare(projectName, path, false, true);
        var fullPath = context.FullPath;     
        _validationService.ValidatePrepToSave(path, fullPath, content, true);     

        // Create directory if needed
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {          
          Directory.CreateDirectory(directory);          
        }

        if (File.Exists(fullPath)) {
          context.Project.CopyToBackup(fullPath);
          _logger.LogInformation("Backup created for file being overwritten: {FilePath}", fullPath);
        }

        // Write the file
        await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);

        // Return success info
        var fileInfo = new FileInfo(fullPath);
        var relativePath = fileInfo.FullName[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);

        _logger.LogInformation("File created successfully: {FilePath}", fullPath);
        var opResult = OperationResult.CreateSuccess(
          Cx.InsertFileCmd,
          $"File created successfully: {path}",
          new {
            fileName = fileInfo.Name,
            path = relativePath,
            size = fileInfo.Length,
            created = fileInfo.CreationTime,
          }
        );

        return opResult;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error creating file: {FilePath}", path);
        var opResult = OperationResult.CreateFailure(
          Cx.InsertFileCmd,
          $"Failed to create file: {ex.Message}",
          ex
        );
        return opResult;
      }
    }

    public async Task<OperationResult> UpdateFileAsync(string projectName, string path, string content) {
      try {     
        // Validate inputs
        _validationService.ValidatePath(path);
        _validationService.ValidateContent(content);
        var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false, false);
        var fullPath = context.FullPath;      
           

        // Security validations
        if (!_securityService.IsWriteAllowed(fullPath)) {
          throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
        }

        if (!_securityService.IsWriteContentSizeAllowed(Encoding.UTF8.GetByteCount(content))) {
          throw new ArgumentException("Content size exceeds maximum allowed for write operations");
        }
                
        string backupPath = context.Project.CopyToBackup(fullPath);
        _logger.LogInformation("Backup created for file being overwritten: {FilePath}", fullPath);


        // UpdateClassItem the file
        await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8).ConfigureAwait(false);

        // Return success info
        var fileInfo = new FileInfo(fullPath);
        var relativePath = fileInfo.FullName[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);
        var relativeBackupPath = backupPath?[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);

        var opResult = OperationResult.CreateSuccess(
          Cx.UpdateFileCmd,
          $"File updated successfully: {path}",
          new {
            fileName = fileInfo.Name,
            path = relativePath,
            size = fileInfo.Length,
            modified = fileInfo.LastWriteTime,
            backupCreated = true,
            backupPath = relativeBackupPath
          }
        );

        return opResult;
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(
          Cx.UpdateFileCmd,
          $"Failed to update file: {ex.Message}",
          ex
        );
        return opResult;       
      }
    }

    public Task<OperationResult> DeleteFileAsync(string projectName, string path, bool confirmDeletion = false) {
      try {     
        // SAFETY: Require explicit confirmation
        if (!confirmDeletion) {
          throw new ArgumentException("Deletion requires explicit confirmation. Set confirmDeletion=true to proceed.");
        }
        _validationService.ValidatePath(path);
        var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false, false);
        var fullPath = context.FullPath;     

        // Security validations - delete is most restrictive
        if (!_securityService.IsDeleteAllowed(fullPath)) {
          throw new UnauthorizedAccessException("DeleteClassItem operation not allowed for security reasons");
        }

        string? backupPath = null;
        FileInfo originalFileInfo = new(fullPath);

        string OriginalFileName = originalFileInfo.Name;
        int originalFileSize = (int)originalFileInfo.Length;


        // Create backup if requested (before deletion)
        backupPath = context.Project.CopyToBackup(fullPath);
        _logger.LogInformation("Backup created for file being overwritten: {FilePath}", fullPath);

        // DeleteClassItem the file
        File.Delete(fullPath);

        // Return success info
        var relativePath = originalFileInfo.FullName[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);
        var relativeBackupPath = backupPath?[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar);

        var opResult = OperationResult.CreateSuccess(
          Cx.DeleteFileCmd,
          $"File deleted successfully: {path}",
          new {
            fileName = OriginalFileName,
            path = relativePath,
            deletedSize = originalFileSize,
            deletedAt = DateTime.Now,
            backupCreated = true,
            backupPath = relativeBackupPath
          }
        );

        return Task.FromResult( opResult);
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure( Cx.DeleteFileCmd, $"Failed delete file: {path} {ex.Message}");
        return Task.FromResult(opResult);
      }
    }

  }
}
