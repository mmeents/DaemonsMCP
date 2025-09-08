using System;
using System.Text;
using System.Text.Json.Serialization;

namespace DaemonsMCP.Core.Models {
  public class ClassContent {

    [JsonPropertyName("classId")]
    public int ClassId { get; set; } = 0; // Reference to IndexClassItem.Id

    [JsonPropertyName("className")]
    public string ClassName { get; set; } = "";

    [JsonPropertyName("fileNamePath")]
    public string FileNamePath { get; set; } = "";

    [JsonPropertyName("namespace")] 
    public string Namespace { get; set; } = "";

    [JsonPropertyName("usesClauseAdd")]
    public string UsesClauseAdd { get; set; } = "";  // when adding updating use to add to uses clause

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
      
    

    
    

    
    

    
    

    

  }
}
