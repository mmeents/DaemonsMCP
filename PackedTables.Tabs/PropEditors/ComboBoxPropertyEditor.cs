using PackedTables.Net;
using PackedTableTabs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PackedTableTabs.PropEditors {

  [PropertyEditor(EditorType.ComboBox, ColumnType.String)]
  public partial class ComboBoxPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public ComboBoxPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private string? _originalValue;

    private IComboBoxDataProvider? _dataProvider;
    private bool _isLoading = false;

    public event EventHandler? ValueChanged;

    private FieldModel? _fieldModel;
    public FieldModel? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        }
      }
    }

    // Data provider for getting combo box items
    public IComboBoxDataProvider? DataProvider {
      get => _dataProvider;
      set {
        _dataProvider = value;
        if (_dataProvider != null) {
          _ = LoadItemsAsync(); // Fire and forget
        }
      }
    }

    // ComboBox specific properties
    public List<object> Items { get; set; } = new();
    public string DisplayMember { get; set; } = "";
    public string ValueMember { get; set; } = "";

    public void SetItems(params object[] items) {
      Items.Clear();
      Items.AddRange(items);
      RefreshComboBoxItems();
    }

    public void SetItems(IEnumerable<object> items) {
      Items.Clear();
      Items.AddRange(items);
      RefreshComboBoxItems();
    }

    private void RefreshComboBoxItems() {
      comboBox1.Items.Clear();
      foreach (var item in Items) {
        comboBox1.Items.Add(item);
      }
    }

    private async Task LoadItemsAsync() {
      if (_dataProvider == null || _isLoading) return;

      try {
        _isLoading = true;
        comboBox1.Items.Clear();
        comboBox1.Items.Add("Loading...");
        comboBox1.Enabled = false;

        var items = await _dataProvider.GetItemsAsync(Field);

        // Update on UI thread
        if (InvokeRequired) {
          Invoke(new Action(() => UpdateComboBoxItems(items)));
        } else {
          UpdateComboBoxItems(items);
        }
      } catch (Exception ex) {
        // Handle error - could log this or show in UI
        if (InvokeRequired) {
          Invoke(new Action(() => {
            comboBox1.Items.Clear();
            comboBox1.Items.Add($"Error: {ex.Message}");
          }));
        } else {
          comboBox1.Items.Clear();
          comboBox1.Items.Add($"Error: {ex.Message}");
        }
      } finally {
        _isLoading = false;
        if (InvokeRequired) {
          Invoke(new Action(() => comboBox1.Enabled = this.Enabled));
        } else {
          comboBox1.Enabled = this.Enabled;
        }
      }
    }

    private void UpdateComboBoxItems(IEnumerable<ComboBoxItem> items) {
      comboBox1.Items.Clear();

      // Add empty option for nullable fields
      comboBox1.Items.Add(new ComboBoxItem("", "(None)"));

      foreach (var item in items) {
        comboBox1.Items.Add(item);
      }

      // Restore previous selection if possible
      if (!string.IsNullOrEmpty(_originalValue)) {
        PropertyValue = _originalValue;
      }
    }

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        comboBox1.Left = value + 3;
        comboBox1.Width = this.Width - value - 6;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => comboBox1.SelectedItem?.ToString() ?? "";
      set {
        // First try to find exact match in ComboBoxItem objects
        foreach (var item in comboBox1.Items) {
          if (item is ComboBoxItem comboItem) {
            if (Equals(comboItem.Value?.ToString(), value)) {
              comboBox1.SelectedItem = item;
              return;
            }
          } else if (Equals(item.ToString(), value)) {
            comboBox1.SelectedItem = item;
            return;
          }
        }

        // If no exact match found and we have a data provider, ask it for display text
        if (_dataProvider != null && !string.IsNullOrEmpty(value)) {
          var displayText = _dataProvider.GetDisplayText(value);
          if (!string.IsNullOrEmpty(displayText)) {
            // Add temporary item if it doesn't exist
            var tempItem = new ComboBoxItem(value, displayText);
            comboBox1.Items.Add(tempItem);
            comboBox1.SelectedItem = tempItem;
            return;
          }
        }

        // Fallback - set to null/empty
        comboBox1.SelectedIndex = string.IsNullOrEmpty(value) ? 0 : -1;
      }
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        comboBox1.Enabled = value && !_isLoading;
      }
    }

    private void ComboBox1_SelectedIndexChanged(object? sender, EventArgs e) {
      if (_isLoading) return;
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        ComboBoxItem? selectedItem = comboBox1.SelectedItem as ComboBoxItem;
        Field.Value = selectedItem?.Value ?? "";
        Modified = false;
      }
    }

    public void ResetToField() {
      if (Field != null) {
        var nameSet = false;
        if (ColumnConfig != null) {
          if (ColumnConfig.LabelText != null) {
            PropertyName = ColumnConfig.LabelText;
            nameSet = true;
          }
          if (ColumnConfig.ReadOnly) {
            Enabled = false;            
          } else {
            Enabled = true;
          }
        }
        if (!nameSet) {
          PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        }
        
        _originalValue = Field?.ValueString;

        // Check if we need to set up a data provider from schema
        SetupDataProviderFromSchema();

        // Set the value (this will happen after async load completes if using data provider)
        PropertyValue = _originalValue ?? "";
        if (comboBox1.SelectedText != "") comboBox1.SelectedText = "";
        Modified = false;
      }
    }
    private void SetupDataProviderFromSchema() {
      if (Field?.OwnerRow?.Owner == null) return;

      var column = Field.OwnerRow.Owner.Columns.Values
          .FirstOrDefault(c => c.Id == Field.ColumnId);

      if (column == null) return;

      // Try to get schema and data provider
      var schema = Field.OwnerRow.Owner.GetUISchema();
      var config = schema?.GetConfig(column.ColumnName);

      if (config?.Properties.TryGetValue("DataProvider", out var provider) == true) {
        if (provider is IComboBoxDataProvider dataProvider) {
          DataProvider = dataProvider;
        }
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      comboBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.StandardEditorWhite;      
    }

    // Method to refresh items manually
    public async Task RefreshItemsAsync() {
      if (_dataProvider != null) {
        await LoadItemsAsync();
      }
    }

    // Method to validate current value
    public bool IsCurrentValueValid() {
      if (_dataProvider == null) return true;

      var currentValue = PropertyValue;
      return string.IsNullOrEmpty(currentValue) || _dataProvider.IsValidValue(currentValue);
    }
  }
}
