using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class BaseItemModel {

    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    [JsonPropertyName("parentId")]
    public int ParentId { get; set; } = 0;

    [JsonPropertyName("typeId")]
    public int TypeId { get; set; } = 0;

    [JsonPropertyName("statusId")]
    public int StatusId { get; set; } = 0;

    [JsonPropertyName("rank")]
    public int Rank { get; set; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public string Details { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;

    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; } = DateTime.Now;

    [JsonPropertyName("completed")]
    public DateTime? Completed { get; set; }
            
    [JsonPropertyName("subnodes")]
    public List<Nodes> Subnodes { get; set; } = new List<Nodes>();
  }
}
