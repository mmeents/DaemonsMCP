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
  public partial class GuidPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public GuidPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private string? _originalValue;
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
        textBox1.Width = this.Width - value - btnGenerate.Width - 6; // Leave space for button
        btnGenerate.Left = this.Width - btnGenerate.Width - 4;

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
        btnGenerate.Enabled = value;
      }
    }

    private void BtnGenerate_Click(object? sender, EventArgs e) {
      textBox1.Text = Guid.NewGuid().ToString();
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TextBox1_Leave(object? sender, EventArgs e) {
      ValidateGuid();
    }

    private void ValidateGuid() {
      if (string.IsNullOrWhiteSpace(textBox1.Text)) return;

      try {
        Guid.Parse(textBox1.Text);
        textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
      } catch {
        textBox1.BackColor = Color.FromArgb(255, 230, 230); // Light red for error
      }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        try {
          Field.Value = Guid.Parse(textBox1.Text);
          Modified = false;
        } catch {
          throw new ValidationException($"Invalid GUID format: {textBox1.Text}");
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
        }
        if (!nameSet) {
          PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        }
        textBox1.Text = Field?.ValueString ?? "";
        _originalValue = textBox1.Text;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (!editing) ValidateGuid();
      textBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }

  }
}
