using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class BaseItemModel {
    public int Id { get; set; } = 0;
    public int ParentId { get; set; } = 0;
    public int TypeId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int Rank { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public DateTime? Completed { get; set; }
  }
}
