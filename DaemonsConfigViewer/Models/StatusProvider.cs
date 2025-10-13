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
  /// Provider specifically for Status items using DaemonsMCP status types
  /// </summary>
  public class StatusProvider : IStatusProvider {

    private INodesRepository _nodesRepo;

    public StatusProvider(INodesRepository nodesRepo) {
      _nodesRepo = nodesRepo ?? throw new ArgumentNullException(nameof(nodesRepo));
    }

    /// <summary>
    /// Interface method to get items for the ComboBox
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ComboBoxItem>> GetItemsAsync(FieldModel? field = null) {
      var statusTypes = _nodesRepo.GetItemStatusTypes();
      return statusTypes.Select(status => {
        // Assuming status object has Name and Description properties
        var name = status.Name;
        var description = status.Description;
        return new ComboBoxItem(status.Id, name, description);
      });
    }

    public bool IsValidValue(object? value) {
      // Could add validation logic here
      return !string.IsNullOrEmpty(value?.ToString());
    }

    public string GetDisplayText(object? value) {
      return value?.ToString() ?? "";
    }
  }

  public interface IStatusProvider : IComboBoxDataProvider;

}
  
