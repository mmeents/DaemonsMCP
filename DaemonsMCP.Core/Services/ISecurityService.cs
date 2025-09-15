using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public interface ISecurityService {
    bool IsFileAllowed(string filePath);
    bool IsWriteAllowed(string filePath);
    bool IsDeleteAllowed(string filePath);
    bool IsPathWriteProtected(string filePath);
    bool IsContentSizeAllowed(long contentSize);
    bool IsWriteContentSizeAllowed(long contentSize);
  }
}
