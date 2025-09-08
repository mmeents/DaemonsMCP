using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class MethodContent {

    [JsonPropertyName("methodId")]
    public int MethodId { get; set; } = 0; // Reference to IndexMethodItem.Id

    [JsonPropertyName("classId")]
    public int ClassId { get; set; } = 0; // Reference to IndexClassItem.Id

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = "";

    [JsonPropertyName("className")]
    public string ClassName { get; set; } = "";

    [JsonPropertyName("methodName")]
    public string MethodName { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("usesClauseAdd")]
    public string UsesClauseAdd { get; set; } = ""; // when adding updating use add to Uses clause. 

    [JsonPropertyName("fileNamePath")]
    public string FileNamePath { get; set; } = "";

  }
}
