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

  [PropertyEditor(EditorType.Memo, ColumnType.String)]
  public partial class MemoPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public MemoPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
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

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.ValueString = textBox1.Text;
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
        textBox1.Text = Field?.ValueString;     
        Modified = false;
      } else {
        textBox1.Text = string.Empty;
      }
    }

    public void CommitValue() {
      if (Field != null && textBox1 != null) {
        Field.Value = textBox1.Text;        
      }
    }

    public void RevertValue() {
      ResetToField();
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (textBox1 != null) {
        textBox1.BackColor = editing ?
            PropertiesTabColors.EditingLabelBackground :
            PropertiesTabColors.StandardEditorWhite;
      }
    }

  }
}
