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
using System.Xml.Linq;

namespace PackedTableTabs.PropEditors {
  /// <summary>
  /// Represents a text property editor control that allows editing of a field's value. <see cref="PropertiesTab"/>
  /// </summary>
  /// <remarks>This control is designed to be used within the PropertiesTab.
  /// Connections with associated with a <see cref="FieldModel"/>. It provides functionality to display and modify the field's value, and
  /// to commit changes back to the field model.</remarks>
  public partial class TextPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public TextPropertyEditor() {
      InitializeComponent();
    }
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private string? _originalValue;
    private bool _isEditing;

    public event EventHandler? ValueChanged;
    public event EventHandler? EditingStarted;
    public bool HasUnsavedChanges =>
        _originalValue != textBox1?.Text;

    private FieldModel? _fieldModel;
    public virtual FieldModel? Field {
      get { return _fieldModel; }
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        } else {
          PropertyName = string.Empty;
        }
      }
    }

    public virtual int LabelRight {
      get {
        return lbName.Left + lbName.Width;
      }
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - 6;
      }
    }

    public virtual string PropertyName {
      get { return lbName.Text; }
      set { lbName.Text = value; }
    }
    public string PropertyValue {
      get { return textBox1.Text; }
      set { textBox1.Text = value; }
    }

    private bool modified = false;
    public bool Modified {
      get { return modified; }
      set { modified = value; }
    }

    private bool _enabled = true;
    public new virtual bool Enabled {
      get {
        return _enabled;
      }
      set {
        _enabled = value;
        if (lbName.Enabled != value) lbName.Enabled = value;
        if (textBox1.Enabled != value) textBox1.Enabled = value;
      }
    }


    private void textBox1_TextChanged(object sender, EventArgs e) {
      if (!Modified) {
        Modified = true;
      }
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        try { 
            Field.Value = textBox1.Text;
            _originalValue = textBox1.Text;            
            Modified = false;
        } catch (ValidationException) { 
          textBox1.BackColor = PropertiesTabColors.ValidationError;
          throw;
        }        
      }
    }

    public void ResetToField() {
      if (Field != null) {
        bool nameSet = false;
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
        _originalValue = Field?.ValueString;
        Modified = false;
      } else {
        textBox1.Text = string.Empty;
      }
    }

    public void CommitValue() {
      if (Field != null && textBox1 != null) {
        Field.Value = textBox1.Text;
        _originalValue = textBox1.Text;
      }
    }

    public void RevertValue() {
      ResetToField();
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (textBox1 != null) {
        textBox1.BackColor = editing ?
            PropertiesTabColors.EditingBackground : 
            PropertiesTabColors.StandardEditorWhite;
        
      }
    }

    private void textBox1_Enter(object sender, EventArgs e) {
      if (!_isEditing) {
        _originalValue = textBox1?.Text;
        EditingStarted?.Invoke(this, EventArgs.Empty);
      }
    }
  }
}

