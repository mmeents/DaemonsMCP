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
  public partial class BooleanPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    private bool _isEditing;
    private bool? _originalValue;
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    public BooleanPropertyEditor() {
      InitializeComponent();
    }

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
        checkBox1.Left = value + 3;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => checkBox1.Checked.ToString();
      set => checkBox1.Checked = bool.TryParse(value, out bool result) && result;
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        checkBox1.Enabled = value;
      }
    }

    private void CheckBox1_CheckedChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.Value = checkBox1.Checked;
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
        }
        if (!nameSet) {
          PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        }
        var boolValue = Field?.Value?.AsBoolean() ?? false;
        checkBox1.Checked = boolValue;
        _originalValue = boolValue;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;      
    }
  }

}

