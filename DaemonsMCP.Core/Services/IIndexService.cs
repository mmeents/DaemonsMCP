using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public interface IIndexService: IDisposable {
    public bool Enabled { get; set; }
    public Task<OperationResult> RebuildIndexAsync(bool IsResync = false);
    public IndexStatusResult GetIndexStatus();
    public Task<ProjectIndexModel> ProcessFileAsync(ProjectIndexModel aProjectIndexModel, string filePath, bool IsResync);

    public new void Dispose();

  }
}
