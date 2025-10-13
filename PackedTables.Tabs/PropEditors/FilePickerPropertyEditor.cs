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
  [PropertyEditor(EditorType.FilePicker, ColumnType.String)]
  public partial class FilePickerPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
      
    public FilePickerPropertyEditor() {
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

    // FilePicker specific properties
    public string Filter { get; set; } = "All files (*.*)|*.*";
    public bool CheckFileExists { get; set; } = true;
    public bool SaveMode { get; set; } = false;

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - btnBrowse.Width - 9;
        btnBrowse.Left = this.Width - btnBrowse.Width - 4;
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
        btnBrowse.Enabled = value;
      }
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BtnBrowse_Click(object? sender, EventArgs e) {
      if (SaveMode) {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = Filter;
        saveDialog.FileName = textBox1.Text;

        if (saveDialog.ShowDialog() == DialogResult.OK) {
          textBox1.Text = saveDialog.FileName;
        }
      } else {
        using var openDialog = new OpenFileDialog();
        openDialog.Filter = Filter;
        openDialog.CheckFileExists = CheckFileExists;
        openDialog.FileName = textBox1.Text;

        if (openDialog.ShowDialog() == DialogResult.OK) {
          textBox1.Text = openDialog.FileName;
        }
      }
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
        textBox1.Text = Field?.ValueString ?? "";
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      textBox1.BackColor = editing ?
        PropertiesTabColors.EditingBackground :
        PropertiesTabColors.StandardEditorWhite;
    }
  }
}
