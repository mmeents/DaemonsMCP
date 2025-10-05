using DaemonsMCP.Core.Extensions;
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
  public class ParentItemProvider : IParentItemProvider {

    private readonly INodesRepository _nodesRepo;
    public ParentItemProvider(INodesRepository nodesRepo) {
      _nodesRepo = nodesRepo ?? throw new ArgumentNullException(nameof(nodesRepo));
    }

    public async Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null) {
      int? parentId = field.OwnerRow[Cx.ItemParentCol].Value.AsInt32();
      var item = _nodesRepo.GetNodes(parentId, 0);
      return item.Select(type => {
        var name = type.Name ?? GetPropertyValue(type, "Name")?.ToString() ?? "Unnamed";
        var description =  type.Details ?? GetPropertyValue(type, "Details")?.ToString();
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

  public interface IParentItemProvider : IComboBoxDataProvider;

}
