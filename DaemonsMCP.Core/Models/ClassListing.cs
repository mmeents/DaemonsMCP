using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {

  public class ClassListing {

    [JsonPropertyName("classId")]
    public int ClassId { get; set; }

    [JsonPropertyName("fileId")]
    public int FileId { get; set; }

    [JsonPropertyName("className")]
    public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("fileNamePath")]
    public string FileNamePath { get; set; } = string.Empty;

    [JsonPropertyName("lineStart")]
    public int LineStart { get; set; }

    [JsonPropertyName("lineEnd")]
    public int LineEnd { get; set; }

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; } = DateTime.Now;

  }
}
