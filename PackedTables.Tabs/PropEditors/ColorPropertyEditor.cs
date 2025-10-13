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

  [PropertyEditor(EditorType.Color, ColumnType.String)]
  public partial class ColorPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public ColorPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private Color _currentColor = Color.White;
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
        colorPanel.Left = value + 3;
        btnChoose.Left = colorPanel.Right + 5;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => ColorTranslator.ToHtml(_currentColor);
      set {
        try {
          _currentColor = ColorTranslator.FromHtml(value);
          colorPanel.BackColor = _currentColor;
        } catch {
          _currentColor = Color.White;
          colorPanel.BackColor = _currentColor;
        }
      }
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        colorPanel.Enabled = value;
        btnChoose.Enabled = value;
      }
    }

    private void ColorPanel_Click(object? sender, EventArgs e) {
      ShowColorDialog();
    }

    private void BtnChoose_Click(object? sender, EventArgs e) {
      ShowColorDialog();
    }

    private void ShowColorDialog() {
      using var colorDialog = new ColorDialog();
      colorDialog.Color = _currentColor;
      colorDialog.FullOpen = true;

      if (colorDialog.ShowDialog() == DialogResult.OK) {
        _currentColor = colorDialog.Color;
        colorPanel.BackColor = _currentColor;
        if (!Modified) Modified = true;
        ValueChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.ValueString = ColorTranslator.ToHtml(_currentColor);
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
        PropertyValue = Field?.ValueString ?? "#FFFFFF";
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }
  }
}
