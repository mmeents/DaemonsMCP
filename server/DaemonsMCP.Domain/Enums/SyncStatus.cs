using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Enums {
  public enum SyncStatus {
    Unknown = 0,
    Success = 1,
    Failed = 2,
    InProgress = 3
  }
}
