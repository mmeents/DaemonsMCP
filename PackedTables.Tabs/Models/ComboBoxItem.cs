using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.Models {

  /// <summary>
  /// Represents an item in a combo box
  /// </summary>
  public class ComboBoxItem {
    public object Value { get; set; }
    public string DisplayText { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;

    public ComboBoxItem(object value, string displayText, string? description = null) {
      Value = value;
      DisplayText = displayText;
      Description = description;
    }

    public override string ToString() => DisplayText;
  }

}
