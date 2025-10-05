using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.PropEditors {

  public enum EditorType {
    Text,
    Boolean,
    Numeric,
    DateTime,
    Guid,
    ComboBox,
    Color,
    FilePicker,
    FolderPicker,
    Memo
  }

  /// <summary>
  /// Factory for creating property editors based on column types and metadata
  /// </summary>
  public static class PropertyEditorFactory {

    /// <summary>
    /// Creates an editor based on the column type with automatic type inference
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(FieldModel field, TableUISchema? schema = null) {
      if (field?.OwnerRow?.Owner == null) return null;
      ColumnModel? column = null;
      if (field.ColumnId > 0) { 
        column = field.OwnerRow.Owner.Columns.Values
          .FirstOrDefault(c => c.Id == field.ColumnId);
      }

      if (field.ColumnId > 0 && column == null) return null;

      EditorType editorType;
      ColumnUIConfig? config = null;
      if (schema != null) {
        config = schema.GetConfig(column?.ColumnName ?? "Id");
        editorType = config.EditorType ?? InferEditorType(column?.ColumnType ?? ColumnType.Int32);
      } else {
        editorType = InferEditorType(column?.ColumnType ?? ColumnType.String);
      }
      return CreateEditor(editorType, config, field);
    }

    /// <summary>
    /// Creates an editor of a specific type
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(EditorType editorType, ColumnUIConfig? config, FieldModel? field = null ) {
      IAmAFieldEditor editor = editorType switch {
        EditorType.Boolean => new BooleanPropertyEditor(),
        EditorType.Numeric => new NumericPropertyEditor(),
        EditorType.DateTime => new DateTimePropertyEditor(),
        EditorType.Guid => new GuidPropertyEditor(),
        EditorType.Text => new TextPropertyEditor(),
        EditorType.ComboBox => new ComboBoxPropertyEditor(),
        EditorType.Color => new ColorPropertyEditor(),
        EditorType.FilePicker => new FilePickerPropertyEditor(),
        EditorType.FolderPicker => new FolderPickerPropertyEditor(),
        EditorType.Memo => new MemoPropertyEditor(),
        _ => new TextPropertyEditor()
      };
      if (config != null) { 
        editor.ColumnConfig = config;
      }
      if (editor != null && field != null) {
        editor.Field = field;
      }

      return editor;
    }

    /// <summary>
    /// Infers the best editor type based on column type
    /// </summary>
    public static EditorType InferEditorType(ColumnType columnType) {
      return columnType switch {
        ColumnType.Boolean => EditorType.Boolean,
        ColumnType.Int32 or ColumnType.Int64 or ColumnType.Decimal => EditorType.Numeric,
        ColumnType.DateTime => EditorType.DateTime,
        ColumnType.Guid => EditorType.Guid,
        ColumnType.String => EditorType.Text,
        ColumnType.Bytes => EditorType.Text, // Could be FilePicker in future
        _ => EditorType.Text
      };
    }

    /// <summary>
    /// Gets the recommended height for an editor type
    /// </summary>
    public static int GetRecommendedHeight(EditorType editorType) {
      return editorType switch {
        EditorType.Memo => 100,
        EditorType.Color => 30,
        EditorType.FilePicker => 30,
        _ => 30
      };
    }

    /// <summary>
    /// Registers a custom editor factory function for extensibility
    /// </summary>
    private static readonly Dictionary<EditorType, Func<IAmAFieldEditor>> _customFactories = new();

    public static void RegisterCustomEditor(EditorType editorType, Func<IAmAFieldEditor> factory) {
      _customFactories[editorType] = factory;
    }
  }

  /// <summary>
  /// Attribute for marking editors with their supported column types
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class PropertyEditorAttribute : Attribute {
    public ColumnType[] SupportedTypes { get; }
    public EditorType EditorType { get; }

    public PropertyEditorAttribute(EditorType editorType, params ColumnType[] supportedTypes) {
      EditorType = editorType;
      SupportedTypes = supportedTypes;
    }
  }

 
}
