using PackedTables.Net;
using PackedTableTabs.PropEditors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs {
  /// <summary>
  /// Simple schema configuration for columns
  /// </summary>
  public class ColumnUIConfig {
    public EditorType? EditorType { get; set; }  // null = auto-detect from ColumnType
    public bool ReadOnly { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;   // 0 = use column rank
    public bool Visible { get; set; } = true;

    // Simple properties for now
    public string? LabelText { get; set; } = null;
    public string? Tooltip { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
  }

  /// <summary>
  /// Simple table schema - just a dictionary of column configurations
  /// </summary>
  public class TableUISchema {
    private readonly ConcurrentDictionary<string, ColumnUIConfig> _columnConfigs = new();
    public string TableName { get; }

    public TableUISchema(string tableName) {
      TableName = tableName;
    }

    /// <summary>
    /// Get or create column configuration
    /// </summary>
    public ColumnUIConfig GetConfig(string columnName) {
      return _columnConfigs.GetOrAdd(columnName, _ => new ColumnUIConfig());
    }

    /// <summary>
    /// Fluent configuration method
    /// </summary>
    public ColumnUIConfig ConfigureColumn(string columnName) {
      return GetConfig(columnName);
    }

    /// <summary>
    /// Get all configured columns
    /// </summary>
    public IEnumerable<(string ColumnName, ColumnUIConfig Config)> GetAllConfigs() {
      return _columnConfigs.Select(kvp => (kvp.Key, kvp.Value));
    }
  }

  /// <summary>
  /// Extension method to get UI metadata for a table
  /// </summary>
  public static class TableUIExtensions {
    private static readonly ConcurrentDictionary<string, TableUISchema> _tableSchemas = new();

    /// <summary>
    /// Get UI schema for this table - creates default if doesn't exist
    /// </summary>
    public static TableUISchema GetUISchema(this TableModel table) {
      return _tableSchemas.GetOrAdd(table.Name, tableName => new TableUISchema(tableName));
    }

    /// <summary>
    /// Clear all schemas (for testing)
    /// </summary>
    public static void ClearAllSchemas() {
      _tableSchemas.Clear();
    }
  }

  /// <summary>
  /// Simple fluent extensions for column configuration
  /// </summary>
  public static class ColumnUIConfigExtensions {
    public static ColumnUIConfig UseEditor(this ColumnUIConfig config, EditorType editorType) {
      config.EditorType = editorType;
      return config;
    }

    public static ColumnUIConfig AsReadOnly(this ColumnUIConfig config) {
      config.ReadOnly = true;
      return config;
    }

    public static ColumnUIConfig WithLabelText(this ColumnUIConfig config, string labelText) {
      config.LabelText = labelText;
      return config;
    }

    public static ColumnUIConfig WithOrder(this ColumnUIConfig config, int order) {
      config.DisplayOrder = order;
      return config;
    }

    public static ColumnUIConfig WithTooltip(this ColumnUIConfig config, string tooltip) {
      config.Tooltip = tooltip;
      return config;
    }

    public static ColumnUIConfig Hide(this ColumnUIConfig config) {
      config.Visible = false;
      return config;
    }

    public static ColumnUIConfig SetProperty(this ColumnUIConfig config, string key, object value) {
      config.Properties[key] = value;
      return config;
    }
  }
}
