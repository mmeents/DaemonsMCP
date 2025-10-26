using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities;

public class Setting {
  public int Id { get; private set; }
  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;
  public string? Description { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime ModifiedAt { get; private set; }

  private Setting() { }

  public Setting(string key, string value, string? description = null) {
    Key = key;
    Value = value;
    Description = description;
    CreatedAt = DateTime.UtcNow;
    ModifiedAt = DateTime.UtcNow;
  }

  public void UpdateValue(string value) {
    Value = value;
    ModifiedAt = DateTime.UtcNow;
  }
}
