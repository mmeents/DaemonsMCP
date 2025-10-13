using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables.Net;
using DaemonsMCP.Core.Models;

namespace DaemonsMCP.Core.Repositories {
  public interface INodesRepository {

    public int notStartedStatusId { get; }

    public string StoragePath { get; set; }
    public string StorageFileName { get; set; }

    public PackedTableSet StorageTables { get; set; }

    public TableModel TypesTable { get; set; }
    public TableModel ItemsTable { get; set; }

    public event Action OnNodesLoadedEvent;
    public void WriteStorage();
    public TableModel MakeTypesTable();

    public string? GetTypeNameById(int typeId);
    public BaseTypeModel? GetTypeById(int typeId);
    public BaseTypeModel InsertTypeItem(BaseTypeModel typeItem);

    public void UpdateTypeItem(BaseTypeModel typeItem);
    public List<StatusType> GetItemStatusTypes();

    public StatusType? GetStatusTypeByName(string? name);

    public StatusType AddUpdateStatusType(StatusType statusType);

    public List<ItemType> GetItemTypes();

    public ItemType? GetItemTypeByName(string name);

    public ItemType AddUpdateItemType(ItemType itemType);    
    
    public Nodes? GetNodesFromItemById(int id);

    public BaseItemModel InsertItem(BaseItemModel item);

    public void UpdateItem(BaseItemModel item);



    public Nodes? GetNodesById(int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public List<Nodes> GetNodes(int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public Nodes AddUpdateNode(Nodes node);

    public bool RemoveNode(int nodeId, RemoveStrategy strategy = RemoveStrategy.PreventIfHasChildren);

    public void CleanupUnusedTypes();
        
    
    public Nodes MakeTodoList(string listName, string[] items);

    public Nodes? GetNextTodoItem(string listName, int maxDepth = 1);

    public Nodes MarkTodoDone(int itemId);

    public Nodes RestoreAsTodo(int itemId);

    public Nodes MarkTodoCancel(int itemId);

  }
}
