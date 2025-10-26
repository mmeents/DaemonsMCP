using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class FileSystemNode {
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public int? ParentId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string RelativePath { get; private set; } = string.Empty;
    public bool IsDirectory { get; private set; }

    // Metadata
    public long? SizeInBytes { get; private set; }
    public string? Extension { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public DateTime IndexedAt { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public FileSystemNode? Parent { get; private set; }
    public ICollection<FileSystemNode> Children { get; private set; } = new List<FileSystemNode>();

    // EF Core constructor
    private FileSystemNode() { }

    // Factory methods
    public static FileSystemNode CreateDirectory(int projectId, string name, string relativePath, int? parentId = null) {
      return new FileSystemNode {
        ProjectId = projectId,
        ParentId = parentId,
        Name = name,
        RelativePath = relativePath,
        IsDirectory = true,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow,
        IndexedAt = DateTime.UtcNow
      };
    }

    public static FileSystemNode CreateFile(int projectId, string name, string relativePath,
        long sizeInBytes, string extension, int? parentId = null) {
      return new FileSystemNode {
        ProjectId = projectId,
        ParentId = parentId,
        Name = name,
        RelativePath = relativePath,
        IsDirectory = false,
        SizeInBytes = sizeInBytes,
        Extension = extension,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow,
        IndexedAt = DateTime.UtcNow
      };
    }

    public void UpdateMetadata(long? sizeInBytes, DateTime modifiedAt) {
      if (!IsDirectory) {
        SizeInBytes = sizeInBytes;
      }
      ModifiedAt = modifiedAt;
      IndexedAt = DateTime.UtcNow;
    }

    public void MoveTo(int? newParentId, string newRelativePath) {
      ParentId = newParentId;
      RelativePath = newRelativePath;
      IndexedAt = DateTime.UtcNow;
    }
  }
}
