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
  public partial class NumericPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public NumericPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private string? _originalValue;
    private ColumnType _columnType;
    public event EventHandler? ValueChanged;

    private FieldModel? _fieldModel;
    public FieldModel? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          _columnType = value.ValueType;
          ResetToField();
        }
      }
    }

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - 6;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => textBox1.Text;
      set => textBox1.Text = value;
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        textBox1.Enabled = value;
      }
    }

    private void TextBox1_KeyPress(object? sender, KeyPressEventArgs e) {
      // Allow control keys (backspace, delete, etc.)
      if (char.IsControl(e.KeyChar)) return;

      switch (_columnType) {
        case ColumnType.Int32:
        case ColumnType.Int64:
          // Allow digits, minus sign at start
          if (!char.IsDigit(e.KeyChar) &&
              !(e.KeyChar == '-' && textBox1.SelectionStart == 0 && !textBox1.Text.Contains('-'))) {
            e.Handled = true;
          }
          break;

        case ColumnType.Decimal:
          // Allow digits, minus sign at start, one decimal point
          if (!char.IsDigit(e.KeyChar) &&
              !(e.KeyChar == '-' && textBox1.SelectionStart == 0 && !textBox1.Text.Contains('-')) &&
              !(e.KeyChar == '.' && !textBox1.Text.Contains('.'))) {
            e.Handled = true;
          }
          break;
      }
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TextBox1_Leave(object? sender, EventArgs e) {
      ValidateAndFormat();
    }

    private void ValidateAndFormat() {
      if (string.IsNullOrWhiteSpace(textBox1.Text)) return;

      try {
        switch (_columnType) {
          case ColumnType.Int32:
            var int32Value = int.Parse(textBox1.Text);
            textBox1.Text = int32Value.ToString();
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;

          case ColumnType.Int64:
            var int64Value = long.Parse(textBox1.Text);
            textBox1.Text = int64Value.ToString();
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;

          case ColumnType.Decimal:
            var decimalValue = decimal.Parse(textBox1.Text);
            textBox1.Text = decimalValue.ToString("F2");
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;
        }
      } catch {
        textBox1.BackColor = Color.FromArgb(255, 230, 230); // Light red for error
      }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        try {
          switch (_columnType) {
            case ColumnType.Int32:
              Field.Value = int.Parse(textBox1.Text);
              break;
            case ColumnType.Int64:
              Field.Value = long.Parse(textBox1.Text);
              break;
            case ColumnType.Decimal:
              Field.Value = decimal.Parse(textBox1.Text);
              break;
          }
          Modified = false;
        } catch {
          throw new ValidationException($"Invalid {_columnType} value: {textBox1.Text}");
        }
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
          if (Field.ColumnId == 0) {
            PropertyName = "(Row ID)";
          } else {
            PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
          }
          
        }
        textBox1.Text = Field?.ValueString ?? "";
        _originalValue = textBox1.Text;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (!editing) ValidateAndFormat();
      textBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }
  }
}
