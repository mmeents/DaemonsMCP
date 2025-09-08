using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class BaseTypeModel {
    public int Id { get; set; }
    public int ParentId { get; set; }
    public int TypeEnum { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

  }
}
