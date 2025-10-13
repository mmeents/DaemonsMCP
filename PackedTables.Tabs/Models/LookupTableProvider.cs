using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.Models {

  /// <summary>
  /// Provider that looks up items from another table/service
  /// </summary>
  public class LookupTableProvider : IComboBoxDataProvider {
    private readonly Func<FieldModel?, Task<IEnumerable<ComboBoxItem>>> _lookupFunction;
    private readonly Func<object?, bool> _validationFunction;

    public LookupTableProvider(
        Func<FieldModel?, Task<IEnumerable<ComboBoxItem>>> lookupFunction,
        Func<object?, bool>? validationFunction = null) {
      _lookupFunction = lookupFunction;
      _validationFunction = validationFunction ?? (_ => true);
    }

    public async Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null) {
      return await _lookupFunction(field);
    }

    public bool IsValidValue(object? value) {
      return _validationFunction(value);
    }

    public string GetDisplayText(object? value) {
      return value?.ToString() ?? "";
    }
  }

}
