using PackedTables.Net;
using PackedTableTabs.PropEditors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackedTableTabs {
  public class PropertiesTab : TabPage {
    private string _titleLabel = "Properties";
    public string PageName {
      get { return this.Text; }
      set { this.Text = value; }
    }
    public PropertiesTab() {
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true
      };
      this.Controls.Add(BasePanel);
      _titleLabel = "Properties";
    }
    public PropertiesTab(string tabName) : base(tabName) {
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true
      };
      this.Controls.Add(BasePanel);
      _titleLabel = tabName;
    }
    private Button? OkButton { get; set; }
    private Button? CancelButton { get; set; }

    private bool _isEditing = false;
    private Panel BasePanel { get; set; }
    private Panel? TitlePanel { get; set; }
    private Panel? MenuPanel { get; set; }
    private ConcurrentDictionary<int, IAmAFieldEditor> _propertyEditors = new ConcurrentDictionary<int, IAmAFieldEditor>();
    private TableUISchema? _tableUISchema;

    private Label? TitleLabel { get; set; }
    private RowModel? _tableRow;
    public RowModel? TableRow {
      get { return _tableRow; }
      set {
        _tableRow = value;
        if (value != null) {
          ResetToRow();
        } else {
          if (TitleLabel != null) TitleLabel.Text = "";
        }
      }
    }

    public MessageLogTab? LogTab { get; set; }

    private void LogMessage(string message) { 
      if (LogTab != null) {
          LogTab.LogMsg(message);
      } else {
          MessageBox.Show(message, "Log Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private bool _disabled = false;
    private int _labelRight = 50; // Default label right position

    //public event EventHandler<RowEditEventArgs>? RowEditStarted;
    //public event EventHandler<RowEditEventArgs>? RowEditCompleted;

    public virtual bool Disabled {
      get { return _disabled; }
      set {
        _disabled = value;
        SetDisabled(value);
      }
    }

    private void SetDisabled(bool disabled) {
      _disabled = disabled;
      if (_disabled) {
        BasePanel.Enabled = false;
        if (TitleLabel != null) TitleLabel.Text = $"{_titleLabel} (Disabled)";
        if (_propertyEditors.Keys.Count > 0) {
          foreach (var editor in _propertyEditors.Values) {
            editor.Enabled = false;
          }
        }
      } else {
        BasePanel.Enabled = true;
        if (TitleLabel != null) TitleLabel.Text = $"{_titleLabel}";
      }
    }

    public event Action OnPostEvent = delegate { };

    private void DoOnPostEvent() {
      if (OnPostEvent != null) {
        OnPostEvent();
      }
    }

    private void OkButton_Click(object? sender, EventArgs e) {
      if (TableRow?.Owner != null) {
        try {
          if (TableRow.Owner.FindFirst("Id", TableRow.Id)) {
            TableRow.Owner.Edit();
          } 
          foreach (var field in TableRow.RowFields.OrderByDescending(kvp => kvp.Key)) {      
            if (!_propertyEditors.ContainsKey(field.Key)) continue; // hidden fields have no editor.
            var editor = _propertyEditors[field.Key];
            if ( editor.Modified) { 
              editor.CommitToField(); // Commit changes to the field
              editor.Modified = false; // Reset modified state
            }
          }
          if (TableRow.Owner.State != TableState.Browse) {
            TableRow.Owner.Post();
            DoOnPostEvent();
          }
          LogMessage("Properties updated successfully.");
          SetEditingMode(false);

        } catch (ValidationException vex) {
          MessageBox.Show($"Validation failed: {vex.Message}", "Validation Error",
              MessageBoxButtons.OK, MessageBoxIcon.Warning);
          LogMessage($"Validation failed: {vex.Message}");
        } catch (Exception ex) {
          LogMessage($"Non Validation Fail: {ex.Message}");
        }
      }
    }

    private void CancelButton_Click(object? sender, EventArgs e) {
      if (TableRow?.Owner != null) {        
        SetEditingMode(false);
        ResetToRow(); // Refresh display
      }
    }

    /// <summary>
    /// Optional: Set a custom UI schema for this properties tab
    /// </summary>
    public TableUISchema? UISchema {
      get => _tableUISchema;
      set => _tableUISchema = value;
    }

    /// <summary>
    /// Builds all the editors for the current row.
    /// </summary>
    public void ResetToRow() {
      if (TableRow == null) {
          if (TitleLabel != null) TitleLabel.Text = "";
          return;
      }      
      BasePanel.Controls.Clear();
      _propertyEditors.Clear();
      var schema = _tableUISchema ?? TableRow.Owner?.GetUISchema();
      MenuPanel = new Panel {
        Dock = DockStyle.Top,
        Height = 24
      };
      BasePanel.Controls.Add(MenuPanel);
      var fieldsToDisplay = GetFieldsToDisplay(TableRow, schema);
      foreach (var field in fieldsToDisplay) {
        var propertyEditor = PropertyEditorFactory.CreateEditor(field, schema);
        if (propertyEditor != null) {        
          propertyEditor.Field = field;
          ((UserControl)propertyEditor).Dock = DockStyle.Top;          
          propertyEditor.LabelRight = 80; // Set a default label right position
          propertyEditor.ValueChanged += PropertyEditor_ValueChanged;
          _propertyEditors[field.Id] = propertyEditor;
          BasePanel.Controls.Add(((UserControl)propertyEditor));          
        }
      }

      _titleLabel = TableRow.Owner?.Name ?? "";
      TitlePanel = new Panel {
        Dock = DockStyle.Top,
        Height = 30,
        BackColor = System.Drawing.Color.LightGray
      };
      // Initialize the properties tab with a label
      TitleLabel = new Label {
        Text = _titleLabel,
        TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      };

      TitlePanel.Controls.Add(TitleLabel);
      BasePanel.Controls.Add(TitlePanel);
      CreateMenuButtons();

      
    }

    /// <summary>
    /// Gets fields to display, respecting schema visibility and ordering
    /// </summary>
    private IEnumerable<FieldModel> GetFieldsToDisplay(RowModel row, TableUISchema? schema) {
      var fields = row.RowFields.Values.ToList();
      fields.Add(row.RowId);

      if (schema == null) {
        // No schema - just return fields in reverse order (existing behavior)
        return fields.OrderByDescending(f => f.Id);
      }

      // Apply schema ordering and visibility, note reverse order to work
      // with DockStyle.Top stacking
      var orderedFields = fields
          .Select(field => {
            var column = row.Owner?.Columns.Values.FirstOrDefault(c => c.Id == field.ColumnId);
            var columnName = column?.ColumnName ?? "";
            var config = schema.GetConfig(columnName);
            return new { Field = field, Config = config, ColumnName = columnName };
          })
          .Where(item => item.Config.Visible)  // Only show visible fields
          .OrderByDescending(item => item.Config.DisplayOrder != 0 ? item.Config.DisplayOrder : item.Field.Id)
          .Select(item => item.Field);

      return orderedFields;
    }

    /// <summary>
    /// Gets editor height based on field and schema configuration
    /// </summary>
    private int GetEditorHeight(FieldModel field, TableUISchema? schema) {
      if (schema != null) {
        var column = field.OwnerRow?.Owner?.Columns.Values.FirstOrDefault(c => c.Id == field.ColumnId);
        if (column != null) {
          var config = schema.GetConfig(column.ColumnName);
          if (config.EditorType.HasValue) {
            return PropertyEditorFactory.GetRecommendedHeight(config.EditorType.Value);
          }
        }
      }

      // Default height
      return 30;
    }

    private void PropertyEditor_ValueChanged(object? sender, EventArgs e) {
      if (!_isEditing) { 
        SetEditingMode(true); // Enable editing mode if a property changes
      }      
    }

    private void CreateMenuButtons() {
      if (MenuPanel == null) return;

      MenuPanel.Controls.Clear();
      int left = _labelRight;
      // OK Button
      OkButton = new Button {
        Text = "OK",
        Size = new Size(50, 20),
        Location = new Point(left+2, 2),
        Enabled = false, // Start disabled
      };
      OkButton.Click += OkButton_Click;

      // Cancel Button  
      CancelButton = new Button {
        Text = "Cancel",
        Size = new Size(50, 20),
        Location = new Point(left+OkButton.Width+ 7, 2),
        Enabled = false, // Start disabled
      };
      CancelButton.Click += CancelButton_Click;

      MenuPanel.Controls.Add(OkButton);
      MenuPanel.Controls.Add(CancelButton);
      if (MenuPanel.Visible) MenuPanel.Visible = false;
    }

    /// <summary>
    /// Sets the right alignment position for labels and adjusts related UI elements accordingly.
    /// </summary>
    /// <remarks>This method updates the right position of labels for all property editors. If the menu panel
    /// and buttons are present, it also adjusts the positions of the OK and Cancel buttons based on the specified right
    /// alignment.</remarks>
    /// <param name="right">The right alignment position for the labels.</param>
    public void SetLabelRight(int right) {
      _labelRight = right;
      if (_propertyEditors.Count > 0) {        
        foreach (var editor in _propertyEditors.Values) {
          editor.LabelRight = right;
        }
      }
      if (MenuPanel != null && OkButton != null && CancelButton != null) {
        OkButton.Left = _labelRight + 2;
        CancelButton.Left = OkButton.Left + OkButton.Width + 5;
      }
    }

    public void SetEditingMode(bool editing) {
      _isEditing = editing;
      if ( MenuPanel == null ) {
        return;          
      }
      if (!MenuPanel.Visible) MenuPanel.Visible = true;
      if (OkButton != null) OkButton.Enabled = editing;
      if (CancelButton != null) CancelButton.Enabled = editing;
      if (MenuPanel.Visible != editing) MenuPanel.Visible = editing;

      // Update visual state of all editors
      foreach (var editor in _propertyEditors.Values) {
        if (editor is IEditStateAware editAware) {
          editAware.SetEditingState(editing);
        }
      }

      // Update title to show state
      if (TitleLabel != null) {
        TitleLabel.Text = editing ? $"{_titleLabel} (Editing)" : _titleLabel;     
      }
    }

    /// <summary>
    /// Validates all fields before allowing commit
    /// </summary>
    public bool ValidateAllFields() {
      var validationErrors = new List<string>();

      foreach (var editor in _propertyEditors.Values) {
        try {
          if (editor.Modified) {
            // Test commit without actually committing
            var tempField = editor.Field;
            if (tempField != null) {
              // This would validate the editor's current value
              editor.CommitToField();
              // If we get here, it's valid, so reset modified state temporarily
              // Real commit happens in OkButton_Click
            }
          }
        } catch (ValidationException vex) {
          validationErrors.Add($"{editor.PropertyName}: {vex.Message}");
        } catch (Exception ex) {
          validationErrors.Add($"{editor.PropertyName}: {ex.Message}");
        }
      }

      if (validationErrors.Any()) {
        var errorMessage = "Validation errors:\n" + string.Join("\n", validationErrors);
        MessageBox.Show(errorMessage, "Validation Failed",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Gets the currently modified editors
    /// </summary>
    public IEnumerable<IAmAFieldEditor> GetModifiedEditors() {
      return _propertyEditors.Values.Where(e => e.Modified);
    }

    /// <summary>
    /// Resets all editors to their original field values
    /// </summary>
    public void ResetAllEditors() {
      foreach (var editor in _propertyEditors.Values) {
        editor.ResetToField();
        editor.Modified = false;
      }
    }

  }

  public static class PropertiesTabColors {
    public static readonly Color StandardEditorWhite = Color.White;
    public static readonly Color NormalBackground = Color.FromKnownColor(KnownColor.Control);
    public static readonly Color EditingBackground = Color.FromArgb(244, 245, 220);
    
    public static readonly Color LabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color EditingLabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color DisabledBackground = Color.LightGray;
    public static readonly Color ValidationError = Color.FromArgb(255, 230, 230);
  }
}
