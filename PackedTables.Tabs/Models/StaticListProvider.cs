using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.Models {
  /// <summary>
  /// Simple static list provider
  /// </summary>
  public class StaticListProvider : IComboBoxDataProvider {

    private readonly List<ComboBoxItem> _items;

    public StaticListProvider(params string[] items) {
      _items = items.Select(item => new ComboBoxItem(item, item)).ToList();
    }

    public StaticListProvider(IEnumerable<ComboBoxItem> items) {
      _items = items.ToList();
    }

    public Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null) {
      return Task.FromResult<IEnumerable<ComboBoxItem>>(_items);
    }

    public bool IsValidValue(object? value) {
      return _items.Any(item => Equals(item.Value, value));
    }

    public string GetDisplayText(object? value) {
      var item = _items.FirstOrDefault(i => Equals(i.Value, value));
      return item?.DisplayText ?? value?.ToString() ?? "";
    }
  }

}
