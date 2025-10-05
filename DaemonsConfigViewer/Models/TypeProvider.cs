using DaemonsMCP.Core.Repositories;
using PackedTables.Net;
using PackedTableTabs;
using PackedTableTabs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsConfigViewer.Models {
  /// <summary>
  /// Provider specifically for Type items using DaemonsMCP item types
  /// </summary>
  public class TypeProvider : ITypeProvider {
    
    private readonly INodesRepository _nodesRepo;
    public TypeProvider(INodesRepository nodesRepo) {
      _nodesRepo = nodesRepo ?? throw new ArgumentNullException(nameof(nodesRepo));
    }

    public async Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null) {
      var itemTypes = _nodesRepo.GetItemTypes();
      return itemTypes.Select(type => {
        var name = type.Name ?? GetPropertyValue(type, "Name")?.ToString() ?? "";
        var description = type.Description ?? GetPropertyValue(type, "Description")?.ToString();
        return new ComboBoxItem(type.Id, name, description);
      });
    }

    public bool IsValidValue(object? value) {
      return !string.IsNullOrEmpty(value?.ToString());
    }

    public string GetDisplayText(object? value) {
      return value?.ToString() ?? "";
    }

    private object? GetPropertyValue(object obj, string propertyName) {
      return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
  }
  public interface ITypeProvider : IComboBoxDataProvider;
}
