using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables.Net;
using PackedTableTabs.Models;

namespace PackedTableTabs {
  /// <summary>
  /// Interface for a field viewer/editor.
  /// </summary>
  public interface IAmAFieldViewer {
    public FieldModel? Field { get; set; }
    public int LabelRight { get; set; }
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }
    public bool Modified { get; set; }
    public bool Enabled { get; set; }
    public void ResetToField();

  }

  public interface IAmAFieldEditor : IAmAFieldViewer {

    public ColumnUIConfig? ColumnConfig { get; set; }

    public event EventHandler? ValueChanged;
    public void CommitToField();

  }

  public interface IEditStateAware {
    void SetEditingState(bool editing);
  }

  /// <summary>
  /// Interface for providing data to combo box editors
  /// </summary>
  public interface IComboBoxDataProvider {
    /// <summary>
    /// Get the list of items for the combo box
    /// </summary>
    Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null);

    /// <summary>
    /// Validate if a value is valid for this provider
    /// </summary>
    bool IsValidValue(object? value);

    /// <summary>
    /// Get display text for a value
    /// </summary>
    string GetDisplayText(object? value);
  }

}
