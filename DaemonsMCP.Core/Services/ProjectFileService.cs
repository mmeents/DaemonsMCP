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
      
      var context = _validationService.ValidateAndPrepare(projectName, path ?? "", true);
      var fullPath = context.FullPath+ Path.DirectorySeparatorChar;
            
      var searchPattern = string.IsNullOrEmpty(filter) ? "*" : filter;

      var files = Directory.GetFiles(fullPath, searchPattern, SearchOption.TopDirectoryOnly)
        .Where(file => _securityService.IsFileAllowed(file))
        .Select(file => file[context.Project.Path.Length..].TrimStart(Path.DirectorySeparatorChar));

      return Task.FromResult(files);      

    }

    public async Task<FileContent> GetFileAsync(string projectName, string path) {

      _validationService.ValidatePath(path);

      var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false );
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


    public async Task<OperationResult> CreateFileAsync(string projectName, string content, string path="", bool createDirectories = true, bool overwrite = false) {
      try {
        _validationService.ValidatePath(path);
        _validationService.ValidateContent(content);      
        var context = _validationService.ValidateAndPrepare(projectName, path, false);
        var fullPath = context.FullPath;     
        _validationService.ValidatePrepToSave(path, fullPath, content, overwrite);     

        // Create directory if needed
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          if (createDirectories) {
            Directory.CreateDirectory(directory);
          } else {
            throw new DirectoryNotFoundException($"Directory does not exist: {Path.GetDirectoryName(path)}");
          }
        }

        if (File.Exists(fullPath) && overwrite) {
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

    public async Task<OperationResult> UpdateFileAsync(string projectName, string path, string content, bool createBackup = true) {
      try {     
        // Validate inputs
        _validationService.ValidatePath(path);
        _validationService.ValidateContent(content);
        var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false);
        var fullPath = context.FullPath;      
           

        // Security validations
        if (!_securityService.IsWriteAllowed(fullPath)) {
          throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
        }

        if (!_securityService.IsWriteContentSizeAllowed(Encoding.UTF8.GetByteCount(content))) {
          throw new ArgumentException("Content size exceeds maximum allowed for write operations");
        }

        string? backupPath = null;
        // Create backup if requested
        if (createBackup) {          
          backupPath = fullPath + $".backup.{DateTime.Now:yyyyMMdd_HHmmss}";
          File.Copy(fullPath, backupPath, true);
        }

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
            backupCreated = createBackup,
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

    public Task<OperationResult> DeleteFileAsync(string projectName, string path, bool createBackup = true, bool confirmDeletion = false) {
      try {     
        // SAFETY: Require explicit confirmation
        if (!confirmDeletion) {
          throw new ArgumentException("Deletion requires explicit confirmation. Set confirmDeletion=true to proceed.");
        }
        _validationService.ValidatePath(path);
        var context = _validationService.ValidateAndPrepare(projectName, path ?? "", false);
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
        if (createBackup) {
          var backupSuffix = $".deleted.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
          backupPath = fullPath + backupSuffix;
          File.Copy(fullPath, backupPath, true);
        }

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
            backupCreated = createBackup,
            backupPath = relativeBackupPath
          }
        );

        return Task.FromResult( opResult);
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is FileNotFoundException)) {
        var opResult = OperationResult.CreateFailure(
         Cx.DeleteFileCmd,
         $"Failed delete file: {path} {ex.Message}",
         ex
        );
        return Task.FromResult(opResult);
      }
    }

  }
}
