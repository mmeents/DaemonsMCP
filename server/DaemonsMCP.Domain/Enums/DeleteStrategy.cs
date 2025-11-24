using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Enums {
  public enum DeleteStrategy {
    PreventIfHasChildren = 0,
    DeleteCascade = 1,
    OrphanChildren = 2,
    ReparentToGrandparent = 3
  }
}
