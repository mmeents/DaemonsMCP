using PackedTableTabs.Models;
using PackedTableTabs.PropEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.Extensions {
  /// <summary>
  /// Extensions for schema configuration with combo box data providers
  /// </summary>
  public static class ComboBoxSchemaExtensions {
    public static ColumnUIConfig WithStaticItems(this ColumnUIConfig config, params string[] items) {
      config.UseEditor(EditorType.ComboBox);
      config.SetProperty("DataProvider", new StaticListProvider(items));
      return config;
    }

    public static ColumnUIConfig WithStaticItems(this ColumnUIConfig config, IEnumerable<ComboBoxItem> items) {
      config.UseEditor(EditorType.ComboBox);
      config.SetProperty("DataProvider", new StaticListProvider(items));
      return config;
    }

    public static ColumnUIConfig WithLookupProvider(this ColumnUIConfig config, IComboBoxDataProvider provider) {
      config.UseEditor(EditorType.ComboBox);
      config.SetProperty("DataProvider", provider);
      return config;
    }

  }
}
