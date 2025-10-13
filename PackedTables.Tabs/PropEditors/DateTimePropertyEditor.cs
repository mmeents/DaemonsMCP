using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackedTableTabs.PropEditors {
  public partial class DateTimePropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public DateTimePropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private bool _isNull;
    private DateTime? _originalValue;

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

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        checkBoxHasValue.Left = value + 3;
        dateTimePicker1.Left = checkBoxHasValue.Right + 5;
        dateTimePicker1.Width = this.Width - dateTimePicker1.Left - 6;

      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get {
        if (_isNull || !checkBoxHasValue.Checked) {
          return string.Empty; // Return empty string for null values
        }
        return dateTimePicker1.Value.ToString("o"); // ISO 8601 format
      }
      set {
        if (string.IsNullOrEmpty(value)) {
          // Handle null/empty as no value
          _isNull = true;
          checkBoxHasValue.Checked = false;
          dateTimePicker1.Enabled = false;
        } else if (DateTime.TryParse(value, out DateTime result)) {
          _isNull = false;
          checkBoxHasValue.Checked = true;
          dateTimePicker1.Value = result;
          dateTimePicker1.Enabled = true;
        }
      }
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        checkBoxHasValue.Enabled = value;
        dateTimePicker1.Enabled = value && checkBoxHasValue.Checked;
      }
    }
    private void CheckBoxHasValue_CheckedChanged(object? sender, EventArgs e) {
      _isNull = !checkBoxHasValue.Checked;
      dateTimePicker1.Enabled = checkBoxHasValue.Checked && this.Enabled;

      if (checkBoxHasValue.Checked && _isNull) {
        // When enabling, set to a reasonable default
        dateTimePicker1.Value = DateTime.Today;
      }

      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DateTimePicker1_ValueChanged(object? sender, EventArgs e) {
      if (checkBoxHasValue.Checked) {
        _isNull = false;
        if (!Modified) Modified = true;
        ValueChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        if (_isNull || !checkBoxHasValue.Checked) {
          Field.Value = null; // Set null value
        } else {
          Field.Value = dateTimePicker1.Value;
        }
        Modified = false;
      }
    }

    public void ResetToField() {
      if (Field != null) {
        try {
          var nameSet = false;
          if (ColumnConfig != null) {
            if (ColumnConfig.LabelText != null) {
              PropertyName = ColumnConfig.LabelText;
              nameSet = true;
            }
          }
          if (!nameSet) {
            PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
          }

          var dateValue = Field?.Value?.AsDateTime();
          if (dateValue.HasValue) {
            _originalValue = dateValue.Value;
            _isNull = false;
            checkBoxHasValue.Checked = true;
            dateTimePicker1.Value = dateValue.Value;
            dateTimePicker1.Enabled = this.Enabled;
          } else {
            // Handle null value
            _originalValue = null;
            _isNull = true;
            checkBoxHasValue.Checked = false;
            dateTimePicker1.Value = DateTime.Today; // Set a default but keep it disabled
            dateTimePicker1.Enabled = false;
          }

          Modified = false;
        } catch {
          // Fallback for any parsing errors
          _isNull = true;
          checkBoxHasValue.Checked = false;
          dateTimePicker1.Enabled = false;
        }
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      // Update visual feedback
      if (editing) {        
        checkBoxHasValue.BackColor = PropertiesTabColors.EditingBackground;
      } else {        
        checkBoxHasValue.BackColor = PropertiesTabColors.NormalBackground;
      }
    }

    // Method to get the actual nullable DateTime value
    public DateTime? GetNullableValue() {
      if (_isNull || !checkBoxHasValue.Checked) {
        return null;
      }
      return dateTimePicker1.Value;
    }

    // Method to set a nullable DateTime value
    public void SetNullableValue(DateTime? value) {
      if (value.HasValue) {
        _isNull = false;
        checkBoxHasValue.Checked = true;
        dateTimePicker1.Value = value.Value;
        dateTimePicker1.Enabled = this.Enabled;
      } else {
        _isNull = true;
        checkBoxHasValue.Checked = false;
        dateTimePicker1.Enabled = false;
      }
    }
  }
}
