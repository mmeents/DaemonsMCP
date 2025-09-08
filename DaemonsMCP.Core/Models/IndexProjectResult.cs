using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {

  public class IndexProjectResult {
    public IndexProjectResult() {
    }
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = "";    

    [JsonPropertyName("fileCount")]
    public int FileCount { get; set; } = 0;

    [JsonPropertyName("classCount")]
    public int ClassCount { get; set; } = 0;

    [JsonPropertyName("methodCount")]
    public int MethodCount { get; set; } = 0;

    [JsonPropertyName("indexing")]
    public int IndexQueuedCount { get; set; } = 0;


  }

}
