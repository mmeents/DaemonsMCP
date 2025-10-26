using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DaemonsMCP.Domain.Entities {

  public enum IndexQueueStatus {
    Pending = 0,      // Waiting to be processed
    Processing = 1,   // Currently being indexed
    Completed = 2,    // Successfully indexed
    Failed = 3        // Error during indexing
  }

  public class IndexQueue {
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public int FileSystemNodeId { get; private set; }

    public string FilePath { get; private set; } = string.Empty;  // Absolute path
    public IndexQueueStatus Status { get; private set; }

    // Tracking
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navigation
    public Project Project { get; private set; } = null!;
    public FileSystemNode FileSystemNode { get; private set; } = null!;

    // EF Core constructor
    private IndexQueue() { }

    // Factory method
    public static IndexQueue Create(
        int projectId,
        int fileSystemNodeId,
        string filePath) {
      return new IndexQueue {
        ProjectId = projectId,
        FileSystemNodeId = fileSystemNodeId,
        FilePath = filePath,
        Status = IndexQueueStatus.Pending,
        CreatedAt = DateTime.UtcNow
      };
    }

    // Domain methods
    public void StartProcessing() {
      Status = IndexQueueStatus.Processing;
      StartedAt = DateTime.UtcNow;
    }

    public void MarkCompleted() {
      Status = IndexQueueStatus.Completed;
      CompletedAt = DateTime.UtcNow;
      ErrorMessage = null;
    }

    public void MarkFailed(string errorMessage) {
      Status = IndexQueueStatus.Failed;
      CompletedAt = DateTime.UtcNow;
      ErrorMessage = errorMessage;
    }
  }
}