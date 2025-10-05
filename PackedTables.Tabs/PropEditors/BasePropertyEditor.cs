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
  public partial class BasePropertyEditor : UserControl, IAmAFieldViewer {
    public BasePropertyEditor() {
      InitializeComponent();
    }

    private bool _modified = false;
    private FieldModel? _fieldModel;

    public virtual FieldModel? Field {
      get { return _fieldModel; }
      set {
        _fieldModel = value;
        ResetToField();        
      }
    }

    public virtual int LabelRight {
      get {
        return lbName.Left + lbName.Width;
      }
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        lbValue.Left = value + 3;
      }
    }

    public virtual string PropertyName {
      get { return lbName.Text; }
      set { lbName.Text = value; }
    }
        
    public bool Modified {
      get { return _modified; }
      set { _modified = value; }
    }

    public new virtual bool Enabled {
      get {
        return base.Enabled;
      }
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        lbValue.Enabled = value;
      }
    }

    public string PropertyValue { 
      get { return lbValue.Text; } 
      set { lbValue.Text = value; }
    }     

    public void ResetToField() {
      if (Field != null) {
        PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        PropertyValue = Field?.ValueString ?? string.Empty;
      } else {
        PropertyName = string.Empty;
        PropertyValue = string.Empty;
      }
    }
  }
}
