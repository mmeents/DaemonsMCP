using System;
using System.Text;

namespace DaemonsMCP.Core.Models {
  public class ClassContent {    
    public int ClassId { get; set; } = 0; // Reference to IndexClassItem.Id
    public string ClassName { get; set; } = "";
    public string FileNamePath { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string UsesClauseAdd { get; set; } = "";  // when adding updating use to add to uses clause
    public string Content { get; set; } = "";
  }
}
