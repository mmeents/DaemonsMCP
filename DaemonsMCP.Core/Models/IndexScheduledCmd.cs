using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexScheduledCmd {
    public IndexScheduledCmd(long id, IndexOpType opType, IndexProjectItem project) {
        Id = id;
        OpType = opType;
        Project = project ?? throw new ArgumentNullException(nameof(project));
    }
    public long Id { get; set; }
    public IndexOpType OpType { get; set; }
    public IndexProjectItem Project { get; set; }

  }

  public enum IndexOpType {
    WriteIndex
  }
  
}
