using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class MethodContent {
    public int MethodId { get; set; } = 0; // Reference to IndexMethodItem.Id
    public int ClassId { get; set; } = 0; // Reference to IndexClassItem.Id
    public string Namespace { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string Content { get; set; } = "";
    public string UsesClauseAdd { get; set; } = ""; // when adding updating use add to Uses clause. 
    public string FileNamePath { get; set; } = "";

  }
}
