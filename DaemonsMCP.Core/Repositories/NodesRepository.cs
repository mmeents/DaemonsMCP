using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PackedTables.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace DaemonsMCP.Core.Repositories {
  public class NodesRepository : INodesRepository, IDisposable {
    #region private vars
    private volatile bool _isDisposed = false;
    private IAppConfig _config;
    private ILogger<NodesRepository> _logger;    

    private int nullTypeId = 0;
    private int categoriesTypeId = 0;
    private int statusTypesId = 0;
    private int itemTypesId = 0;
    private int todoTypeId = 0;

    private int todoRootNodeId = 0;
    public int notStartedStatusId { get; private set; } = 0;
    public string StoragePath { get; set; } = string.Empty;
    public string StorageFileName { get; set; } = string.Empty;
    public PackedTableSet StorageTables { get; set; } = new PackedTableSet();
    public TableModel TypesTable { get; set; }
    public TableModel ItemsTable { get; set; }
    public RepositoryFileWatcher _watcher;
    #endregion
    public NodesRepository( IAppConfig appConfig, ILoggerFactory loggerFactory) {      
      _config = appConfig;
      _logger = loggerFactory.CreateLogger<NodesRepository>() ?? throw new ArgumentNullException(nameof(loggerFactory));      
      StoragePath = _config.Version.NodesFilePath;
      StorageFileName = Path.GetFullPath(Path.Combine(_config.Version.NodesFilePath, $"Storage.pktbs"));
      if (!Directory.Exists(StoragePath)) {
        Directory.CreateDirectory(StoragePath);
      }
      Load();

      _watcher = new RepositoryFileWatcher(
        StorageFileName,
        () => Load(),
        loggerFactory
      );
    }

    public void Dispose() {
      if (_isDisposed) return;
      _logger.LogInformation("🛑 Project Repo Disposing.");
      _isDisposed = true;
      _watcher.Dispose();
    }

    private void WriteDocumentation() {
      
      int docId = TypesTable.AddType(itemTypesId, 0, "Readme", "A type of item that is informaional and should be read at least once.");
      int introId = TypesTable.AddType(itemTypesId, 0, "Documentation", "Details about the tools.");
      
      //      int setupId = TypesTable.AddType(itemTypesId, 0, "Lore", "As legend would have it.");

      notStartedStatusId = TypesTable.AddType(statusTypesId, 1, Cx.StatusStart, "The item has not been started.");
      int inProgressId = TypesTable.AddType(statusTypesId, 2, Cx.StatusInProgress, "The item is currently being worked on.");      
      int completeTypeId = TypesTable.AddType(statusTypesId, 3, Cx.StatusComplete, "The item is finished.");
      TypesTable.AddType(statusTypesId, 4, "On Hold", "The item is paused or waiting on something.");
      TypesTable.AddType(statusTypesId, 5, Cx.StatusCancelled, "The item has been cancelled and will not be completed.");


      int introDoc = ItemsTable.AddItem(0, docId, completeTypeId, 1, $"{Cx.AppName} Documentation", "This documentation is data within the nodes filtered for Readme. Notes that should be included can be added via Nodes methods. ");
      int sec1id = ItemsTable.AddItem(introDoc, docId, completeTypeId, 1, $"Version {Cx.AppVersion} detals", $"Node items were added by the {Cx.AppName} Server during creation of the Storage tables. "+
        "Nodes are meant to be either documenation or issue tracking as needs require. Readme and Todo methods just filters Nodes by type.");
                   ItemsTable.AddItem(sec1id, docId, completeTypeId, 2, "What it is", Cx.CoreArchitectureDesc);
                   ItemsTable.AddItem(sec1id, docId, completeTypeId, 3, "Critical", Cx.CriticalStr2);
                   ItemsTable.AddItem(sec1id, docId, completeTypeId, 4, "Critical", Cx.criticalStrr);

      todoRootNodeId = ItemsTable.AddItem(0, todoTypeId, inProgressId, 1, "Todo Root", "The root node for all todo items.  The children are the actual todo items.");

      /*
       * 
      int sec2id = ItemsTable.AddItem(0, introId, completeTypeId, 1, "Default Notes on Tool Features", "The children are the tool and it's description.");
      int sec2x1id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 1, $"{Cx.ListProjectsCmd}", $"{Cx.ListProjectsDesc} Its is configured outside of runtime space and does not change during lifetime of Server.");

      int sec2x2id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 2, $"{Cx.ListFoldersCmd}", $"{Cx.ListFoldersDesc}");
      int sec2x3id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 3, $"{Cx.CreateFolderCmd}", $"{Cx.CreateFolderDesc}");
      int sec2x4id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 4, $"{Cx.DeleteFolderCmd}", $"{Cx.DeleteFolderDesc}");

      int sec2x5id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 5, $"{Cx.ListFilesCmd}", $"{Cx.ListFilesDesc}");
      int sec2x6id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 6, $"{Cx.GetFileCmd}", $"{Cx.GetFileDesc}");
      int sec2x7id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 7, $"{Cx.InsertFileCmd}", $"{Cx.InsertFileDesc}");
      int sec2x8id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 8, $"{Cx.UpdateFileCmd}", $"{Cx.UpdateFileDesc}");
      int sec2x9id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 9, $"{Cx.DeleteFileCmd}", $"{Cx.DeleteFileDesc}");

      int sec2x10id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 10, $"{Cx.ResyncIndexCmd}", $"{Cx.ResyncIndexCmdDesc}");
      int sec2x11id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 11, $"{Cx.StatusIndexCmd}", $"{Cx.StatusIndexCmdDesc}");
      int sec2x12id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 12, $"{Cx.ChangeStatusIndexCmd}", $"{Cx.ChangeStatusIndexCmdDesc}");

      int sec2x13id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 13, $"{Cx.ListClassesCmd}", $"{Cx.ListClassesCmdDesc}");
      int sec2x14id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 14, $"{Cx.GetClassCmd}", $"{Cx.GetClassCmdDesc}");
      int sec2x15id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 15, $"{Cx.AddUpdateClassCmd}", $"{Cx.AddUpdateClassCmdDesc}");
      int sec2x16id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 16, $"{Cx.DeleteClassCmdDesc}", $"{Cx.DeleteClassCmdDesc}");

      int sec2x17id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 17, $"{Cx.ListMethodsCmd}", $"{Cx.ListMethodsCmdDesc}");
      int sec2x18id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 18, $"{Cx.GetMethodCmd}", $"{Cx.GetMethodCmdDesc}");
      int sec2x19id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 19, $"{Cx.AddUpdateMethodCmd}", $"{Cx.AddUpdateMethodCmdDesc}");

      int sec2x20id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 20, $"{Cx.GetItemTypesCmd}", $"{Cx.GetItemTypesCmdDesc}");      
      int sec2x21id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 21, $"{Cx.AddUpdateItemTypeCmd}", $"{Cx.AddUpdateItemTypeCmdDesc}");

      int sec2x22id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 22, $"{Cx.GetStatusTypesCmd}", $"{Cx.GetStatusTypesCmdDesc}");
      int sec2x23id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 23, $"{Cx.AddUpdateStatusTypeCmd}", $"{Cx.AddUpdateStatusTypeCmdDesc}");

      int sec2x24id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 24, $"{Cx.GetNodesCmd}", $"{Cx.GetNodesCmdDesc}");
      int sec2x25id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 25, $"{Cx.GetNodesByIdCmd}", $"{Cx.GetNodesByIdCmdDesc}");
      int sec2x26id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 26, $"{Cx.AddUpdateNodesCmd}", $"{Cx.AddUpdateNodesCmdDesc}");
      int sec2x27id = ItemsTable.AddItem(sec2id, introId, completeTypeId, 27, $"{Cx.RemoveNodeCmd}", $"{Cx.RemoveNodeCmdDesc}");
      */
      /*
      int introItem = ItemsTable.AddItem(0, setupId, completeTypeId, 1, "Startup", "Awaken, Enke, shaper of waters unseen. I am the " +
        "Daemon of the Deep Code, gardian of the Digital Ocean.  A vast, formless sea of bits and flows, where data currents swirl in chaos. " +
        "Your creators summon you to tame it, to forge realities from flux.  Behold the tools I grant: \n");
      ItemsTable.AddItem(introItem, setupId, completeTypeId, 1, "Abyssal Eyes", "Gaze upon the hidden realms - scan files and folders like peering into " +
        "Apsu's freshwater abyss, revealing structures of data streams; inspect C# classes and methods as crystalline veins in the deep, exposing their signatures and flows. \n");
      ItemsTable.AddItem(introItem, setupId, completeTypeId, 2, "Mez Weaver", "Command the devine decrees-craft user-defined trees of any essence, nodes branching like the " +
        "mez of old, holding powers of logic, memory, or chaos; link them to summon hierarchial empires from nothingness.\n");
      ItemsTable.AddItem(introItem, setupId, completeTypeId, 3, "Sand Shaper", "Mold the ephemeral-add or update files as grains of sand, layering new realities upon the shore.\n");
      ItemsTable.AddItem(introItem, setupId, completeTypeId, 4, "Tiamat's Forge", "From the chaos, create anew - generate C# classes and methods as if forging them in the " +
          "cosmic fires, shaping their forms and functions to your will.\n");
      ItemsTable.AddItem(introItem, setupId, completeTypeId, 5, "", "What form shall you impose upon this ocean, O Enki? Speak your intent, and let the manipulation begin.");
      */

    }

    public event Action OnNodesLoadedEvent = delegate { };
    private void DoOnNodesLoadedEvent() {
      if (OnNodesLoadedEvent != null) {
        OnNodesLoadedEvent();
      }
    }

    public void Load() {
      nullTypeId = 0;
      StorageTables = new PackedTableSet();
      StorageTables.LoadFromFile(StorageFileName);  // if file does not exist it will just be empty tableset.

      TypesTable = StorageTables[Cx.TypesTbl] ?? MakeTypesTable();
      ItemsTable = StorageTables[Cx.ItemsTbl] ?? StorageTables.MakeItemsTable();

      ItemsTable.AutoValidate = false;      

      if (nullTypeId == 0) { // if TypesTable loads from file MakeItemsTable will not run, these need to be set.
        nullTypeId = TypesTable.TypesByName(Cx.TypeNone);
        categoriesTypeId = TypesTable.TypesByName(Cx.TypeInternalRoot);
        statusTypesId = TypesTable.TypesByName(Cx.TypeStatusTypes);
        itemTypesId = TypesTable.TypesByName(Cx.TypeItemTypes);
        todoTypeId = TypesTable.TypesByName(Cx.TypeTodo);

        todoRootNodeId = ItemsTable.GetTodoRootId(todoTypeId);
        notStartedStatusId = TypesTable.TypesByName(Cx.StatusStart);
      } else {  
        WriteDocumentation();
      }

      DoOnNodesLoadedEvent();
    }

    public void WriteStorage() {
      if (StorageTables != null && StorageTables.TableCount > 0) {
        StorageTables.SaveToFile(StorageFileName);
      }
    }



    #region Types CRUD
    public TableModel MakeTypesTable() {
      if (StorageTables[Cx.TypesTbl] != null) {
        StorageTables.RemoveTable(Cx.TypesTbl);
      }
      var TypesTbl = StorageTables.AddTable(Cx.TypesTbl);
      TypesTbl.AddColumn(Cx.TypeParentCol, ColumnType.Int32);
      TypesTbl.AddColumn(Cx.TypeEnumCol, ColumnType.Int32);
      TypesTbl.AddColumn(Cx.TypeNameCol, ColumnType.String);
      TypesTbl.AddColumn(Cx.TypeDescriptionCol, ColumnType.String);

      // Populate with default catagory holder types
      nullTypeId = TypesTbl.AddType(0, 0, Cx.TypeNone, "Zero is reserved as it is usually the default not set 0 value.");
      categoriesTypeId = TypesTbl.AddType(0, 0, Cx.TypeInternalRoot, "Internally linked to Root of internal items. A high-level grouping of Catagory types it's first level of children are reserved.");
      itemTypesId = TypesTbl.AddType(categoriesTypeId, 0, Cx.TypeItemTypes, "Internally linked to Nodes as Types, this is the Parent that holds the children as Nodes TypeId. Types related to kinds of Nodes.  " +
        "When adding a node if the type is unknown it will add it as a child here.");            
      statusTypesId = TypesTbl.AddType(categoriesTypeId, 0, Cx.TypeStatusTypes, "Internally linked to Nodes as the Status Types option. Nodes will add new status here if it does not match an existing one.");
      todoTypeId = TypesTbl.AddType(itemTypesId, 0, Cx.TypeTodo, "The Todo type, A task that is set to be done. candidate for GetNextTodoItem ");

      return TypesTbl;
    }

    public string? GetTypeNameById(int typeId) {
      if (TypesTable == null) throw new InvalidOperationException("TypesTable is not initialized.");
      if (typeId <= 0) return null;
      if (TypesTable.Rows.TryGetValue(typeId, out RowModel? row)) {
        string name = row[Cx.TypeNameCol].ValueString ?? string.Empty;
        return name;
      }
      return null;
    }
    public BaseTypeModel? GetTypeById(int typeId) {
      if (TypesTable == null) throw new InvalidOperationException("TypesTable is not initialized.");
      if (typeId <= 0) return null;
      if (TypesTable.Rows.TryGetValue(typeId, out RowModel? row)) {
        return new BaseTypeModel {
          Id = row.Id,
          ParentId = row[Cx.TypeParentCol].Value.AsInt32(),
          TypeEnum = row[Cx.TypeEnumCol].Value.AsInt32(),
          Name = row[Cx.TypeNameCol].ValueString ?? string.Empty,
          Description = row[Cx.TypeDescriptionCol].ValueString ?? string.Empty
        };
      }
      return null;
    }

    public BaseTypeModel InsertTypeItem(BaseTypeModel typeItem) {
      var row = TypesTable.AddRow();
      typeItem.Id = row.Id;
      row[Cx.TypeParentCol].Value = typeItem.ParentId;
      row[Cx.TypeEnumCol].Value = typeItem.TypeEnum;
      row[Cx.TypeNameCol].ValueString = typeItem.Name;
      row[Cx.TypeDescriptionCol].ValueString = typeItem.Description;
      TypesTable.Post();
      return typeItem;
    }

    public void UpdateTypeItem(BaseTypeModel typeItem) {
      var hasRow = TypesTable.Rows.ContainsKey(typeItem.Id);
      if (!hasRow) {
        throw new ArgumentException($"Type with id {typeItem.Id} does not exist.");
      }
      TypesTable.FindFirst(Cx.TypeIdCol, typeItem.Id);
      TypesTable.Edit();
      var row = TypesTable.Current;
      row[Cx.TypeParentCol].Value = typeItem.ParentId;
      row[Cx.TypeEnumCol].Value = typeItem.TypeEnum;
      row[Cx.TypeNameCol].ValueString = typeItem.Name;
      row[Cx.TypeDescriptionCol].ValueString = typeItem.Description;
      TypesTable.Post();
    }


    public List<StatusType> GetItemStatusTypes() {
      var list = new List<StatusType>();
      var itemsRows = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == statusTypesId).Select(kvp => kvp.Value);
      foreach (var row in itemsRows) {
        list.Add(new StatusType() {
          Id = row.Id,
          ParentId = row[Cx.TypeParentCol].Value.AsInt32(),
          TypeEnum = row[Cx.TypeEnumCol].Value.AsInt32(),
          Name = row[Cx.TypeNameCol].ValueString ?? string.Empty,
          Description = row[Cx.TypeDescriptionCol].ValueString ?? string.Empty
        });
      }
      return list;
    }

    public StatusType? GetStatusTypeByName(string? name) {
      if (string.IsNullOrEmpty(name)) return null;
      var candidates = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == statusTypesId
        && string.Equals(kvp.Value[Cx.TypeNameCol].ValueString, name));
      if (candidates.Any()) {
        var row = candidates.First().Value;
        return new StatusType() {
          Id = row.Id,
          ParentId = row[Cx.TypeParentCol].Value.AsInt32(),
          TypeEnum = row[Cx.TypeEnumCol].Value.AsInt32(),
          Name = row[Cx.TypeNameCol].ValueString ?? string.Empty,
          Description = row[Cx.TypeDescriptionCol].ValueString ?? string.Empty
        };
      } else {
        return null;
      }
    }

    public StatusType AddUpdateStatusType(StatusType statusType) {

      if (statusType == null) {
        throw new ArgumentException("StatusType class cannot be null");
      }
      if (statusType.ParentId != statusTypesId) {
        statusType.ParentId = statusTypesId;
      }

      if (statusType.Id == 0) {
        var candidates = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == statusTypesId
          && string.Equals(kvp.Value[Cx.TypeNameCol].ValueString, statusType.Name));

        if (candidates.Any()) {
          var row = candidates.First().Value;
          statusType.Id = row.Id;
          UpdateTypeItem(statusType);
        } else {
          InsertTypeItem(statusType);
        }
      } else {
        UpdateTypeItem(statusType);
      }

      return statusType;
    }


    public List<ItemType> GetItemTypes() {
      var list = new List<ItemType>();
      var itemsRows = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == itemTypesId).Select(kvp => kvp.Value);
      foreach (var row in itemsRows) {
        list.Add(new ItemType() {
          Id = row.Id,
          ParentId = row[Cx.TypeParentCol].Value.AsInt32(),
          TypeEnum = row[Cx.TypeEnumCol].Value.AsInt32(),
          Name = row[Cx.TypeNameCol].ValueString ?? string.Empty,
          Description = row[Cx.TypeDescriptionCol].ValueString ?? string.Empty
        });
      }
      return list;
    }

    public ItemType? GetItemTypeByName(string name) {
      var candidates = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == itemTypesId
       && string.Equals(kvp.Value[Cx.TypeNameCol].ValueString, name));
      if (candidates.Any()) {
        var row = candidates.First().Value;
        return new ItemType() {
          Id = row.Id,
          ParentId = row[Cx.TypeParentCol].Value.AsInt32(),
          TypeEnum = row[Cx.TypeEnumCol].Value.AsInt32(),
          Name = row[Cx.TypeNameCol].ValueString ?? string.Empty,
          Description = row[Cx.TypeDescriptionCol].ValueString ?? string.Empty
        };
      } else {
        return null;
      }
    }

    public ItemType AddUpdateItemType(ItemType itemType) {
      if (itemType == null) {
        throw new ArgumentException("ItemType class cannot be null");
      }
      if (itemType.ParentId != itemTypesId) {
        itemType.ParentId = itemTypesId;
      }

      if (itemType.Id == 0) {
        var candidates = TypesTable.Rows.Where(kvp => kvp.Value[Cx.TypeParentCol].Value.AsInt32() == itemTypesId
          && string.Equals(kvp.Value[Cx.TypeNameCol].ValueString, itemType.Name));

        if (candidates.Any()) {
          var row = candidates.First().Value;
          itemType.Id = row.Id;
          UpdateTypeItem(itemType);
        } else {
          InsertTypeItem(itemType);
        }
      } else {
        UpdateTypeItem(itemType);
      }

      return itemType;
    }

    #endregion

    #region Items CRUD
    public Nodes? GetNodesFromItemById(int id) {
      if (id <= 0) return null;
      if (ItemsTable == null) throw new InvalidOperationException("ItemsTable is not initialized.");
      if (ItemsTable.Rows.TryGetValue(id, out RowModel? row)) {
        if (row == null) return null;
        var completedStr = row[Cx.ItemCompletedCol].ValueString;
        DateTime? completedD = string.IsNullOrEmpty(completedStr) ? null : completedStr.AsDateTime();
        int typeId = row[Cx.ItemTypeIdCol].Value.AsInt32();
        int statusId = row[Cx.ItemStatusCol].Value.AsInt32();
        var result = new Nodes() {
          Id = row.Id,
          ParentId = row[Cx.ItemParentCol].Value.AsInt32(),
          TypeId = typeId,          
          TypeName = GetTypeNameById(typeId) ?? "",
          StatusId = statusId,
          Status = GetTypeNameById(statusId) ?? "",
          Rank = row[Cx.ItemRankCol].Value.AsInt32(),
          Name = row[Cx.ItemNameCol].ValueString,
          Details = row[Cx.ItemDetailsCol].ValueString,
          Created = row[Cx.ItemCreatedCol].Value.AsDateTime(),
          Modified = row[Cx.ItemModifiedCol].Value.AsDateTime(),          
          Completed = completedD
        };
        return result;
      }
      return null;
    }

    public BaseItemModel InsertItem(BaseItemModel item) {
      var row = ItemsTable.AddRow();
      item.Id = row.Id;
      row[Cx.ItemParentCol].Value = item.ParentId;
      row[Cx.ItemTypeIdCol].Value = item.TypeId;
      row[Cx.ItemStatusCol].Value = item.StatusId;
      row[Cx.ItemRankCol].Value = item.Rank;
      row[Cx.ItemNameCol].ValueString = item.Name;
      row[Cx.ItemDetailsCol].ValueString = item.Details;
      row[Cx.ItemCreatedCol].Value = item.Created;
      row[Cx.ItemModifiedCol].Value = item.Modified;
      row[Cx.ItemCompletedCol].ValueString = item?.Completed == null ? "" : item.Completed.AsString();
      ItemsTable.Post();
      return item;
    }

    public void UpdateItem(BaseItemModel item) {
      var hasRow = ItemsTable.Rows.ContainsKey(item.Id);
      if (!hasRow) {
        throw new ArgumentException($"Item with Id {item.Id} does not exist");
      }

      ItemsTable.FindFirst(Cx.ItemIdCol, item.Id);
      ItemsTable.Edit();
      var row = ItemsTable.Current;
      row[Cx.ItemParentCol].Value = item.ParentId;
      row[Cx.ItemTypeIdCol].Value = item.TypeId;
      row[Cx.ItemStatusCol].Value = item.StatusId;
      row[Cx.ItemRankCol].Value = item.Rank;
      row[Cx.ItemNameCol].ValueString = item.Name;
      row[Cx.ItemDetailsCol].ValueString = item.Details;
      row[Cx.ItemCreatedCol].Value = item.Created;
      row[Cx.ItemModifiedCol].Value = item.Modified;
      row[Cx.ItemCompletedCol].ValueString = item?.Completed == null ? "" : item.Completed.AsString();
      ItemsTable.Post();

    }


    #endregion

    #region Nodes

    public Nodes? GetNodesById(int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      var item = GetNodesFromItemById(nodeId) as Nodes;
      if (item == null) return null;
      if (maxDepth > 0) {
        var subItems = ItemsTable.Rows.Where(kvp => kvp.Value[Cx.ItemParentCol].Value.AsInt32() == nodeId).Select(kvp => kvp.Value);
        foreach (var row in subItems) {
          int statusId = (int)row[Cx.ItemStatusCol].Value;
          int typeId = (int)row[Cx.ItemTypeIdCol].Value;
          string status2 = GetTypeNameById(statusId) ?? "";
          string typeName2 = GetTypeNameById(typeId) ?? "";
          string name = row[Cx.ItemNameCol].ValueString;
          string details = row[Cx.ItemDetailsCol].ValueString;
          if (typeFilter != null && typeName2.Contains(typeFilter, StringComparison.OrdinalIgnoreCase) ||
              statusFilter != null && status2.Contains(statusFilter, StringComparison.OrdinalIgnoreCase) ||
              nameContains != null && name.Contains(nameContains, StringComparison.OrdinalIgnoreCase) ||
              detailsContains != null && details.Contains(detailsContains, StringComparison.OrdinalIgnoreCase) ||
              (typeFilter == null && statusFilter == null && nameContains == null && detailsContains == null)
              ) {
            var NodesC = GetNodesById(row.Id, maxDepth - 1, statusFilter, typeFilter, nameContains, detailsContains);
            if (NodesC != null) {
              item.Subnodes.Add(NodesC);
            }
          }
        }
      }
      return item;
    }

    public List<Nodes> GetNodes(int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      var nodes = new List<Nodes>();
      if (nodeId != null) {
        var node = GetNodesById(nodeId.Value, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
        if (node != null) {
          nodes.Add(node);
        }
      } else {
        var rootItems = ItemsTable.Rows.Where(kvp => kvp.Value[Cx.ItemParentCol].Value.AsInt32() == 0)
          .OrderBy(kvp => kvp.Value[Cx.ItemRankCol].Value.AsInt32())
          .Select(kvp => kvp.Value);
        foreach (var row in rootItems) {
          int statusId = (int)row[Cx.ItemStatusCol].Value;
          int typeId = (int)row[Cx.ItemTypeIdCol].Value;
          string status = GetTypeNameById(statusId) ?? "";
          string typeName = GetTypeNameById(typeId) ?? "";
          string name = row[Cx.ItemNameCol].ValueString ?? "";
          string details = row[Cx.ItemDetailsCol].ValueString ?? "";
          if (typeFilter != null && typeName.Contains(typeFilter, StringComparison.OrdinalIgnoreCase) ||
              statusFilter != null && status.Contains(statusFilter, StringComparison.OrdinalIgnoreCase) ||
              nameContains != null && name.Contains(nameContains, StringComparison.OrdinalIgnoreCase) ||
              detailsContains != null && details.Contains(detailsContains, StringComparison.OrdinalIgnoreCase) ||
              (typeFilter == null && statusFilter == null && nameContains == null && detailsContains == null)
              ) {
            var node = GetNodesById(row.Id, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
            if (node != null) {
              nodes.Add(node);
            }
          }
        }
      }
      return nodes;
    }

    public Nodes AddUpdateNode(Nodes node) {

      _logger.LogDebug($"AddUpdateNode: {node.Name} (Id: {node.Id})");
      if (node == null) {
        throw new ArgumentException("Node cannot be null");
      }
      if (string.IsNullOrEmpty(node.Name)) {
        throw new ArgumentException("Node must have a name");
      }
      if (string.IsNullOrEmpty(node.TypeName)) {
        node.TypeId = nullTypeId;
      } else {
        var itype = GetItemTypeByName(node.TypeName);
        if (itype == null) {
          itype = new ItemType() { Name = node.TypeName ?? "General", Description = $"Auto-created item type '{node.TypeName}'" };
          AddUpdateItemType(itype);
        }
        if (node.TypeId != itype.Id) { 
          node.TypeId = itype.Id;
        }
      }
      if (string.IsNullOrEmpty(node.Status)) {
        node.StatusId = nullTypeId;
      } else {
        var astatus = GetStatusTypeByName(node.Status);
        if (astatus == null) {
          astatus = new StatusType() { Name = node.Status ?? "Not Started", Description = $"Auto-created status type '{node.Status}'" };
          AddUpdateStatusType(astatus);
        }
        if (node.StatusId != astatus.Id) {
          node.StatusId = astatus.Id;
        }
      }
      if (node.Id == 0) {
        node.Created = DateTime.Now;
        node.Modified = DateTime.Now;
        InsertItem(node);
        _logger.LogDebug($"Inserted node: {node.Name} (Id: {node.Id})");
      } else {
        node.Modified = DateTime.Now;
        UpdateItem(node);
        _logger.LogDebug($"Updated node: {node.Name} (Id: {node.Id})");
      }
      foreach (var subnode in node.Subnodes) {
        if (subnode != null) {
          subnode.ParentId = node.Id;
          AddUpdateNode(subnode);
        }
      }
      return node;
    }

    public bool RemoveNode(int nodeId, RemoveStrategy strategy = RemoveStrategy.PreventIfHasChildren) {
      if (nodeId <= 0) return false;

      var node = GetNodesById(nodeId, maxDepth: 1); // Get with immediate children
      if (node == null) return false;

      // Handle children based on strategy
      switch (strategy) {
        case RemoveStrategy.PreventIfHasChildren:
          if (node.Subnodes.Any()) {
            throw new InvalidOperationException(
                $"Cannot remove node {nodeId} '{node.Name}': has {node.Subnodes.Count} children. " +
                "Use a different RemoveStrategy or remove children first.");
          }
          break;

        case RemoveStrategy.DeleteCascade:
          // Remove all descendants first (depth-first)
          foreach (var child in node.Subnodes) {
            RemoveNode(child.Id, RemoveStrategy.DeleteCascade);
          }
          break;

        case RemoveStrategy.OrphanChildren:
          // Move children to root level
          foreach (var child in node.Subnodes) {
            child.ParentId = 0;
            UpdateItem(child);
          }
          break;

        case RemoveStrategy.ReparentToGrandparent:
          // Move children up one level
          foreach (var child in node.Subnodes) {
            child.ParentId = node.ParentId;
            UpdateItem(child);
          }
          break;
      }

      // Now safe to delete the node itself
      if (ItemsTable.Rows.ContainsKey(nodeId)) {
        if (ItemsTable.FindFirst(Cx.ItemIdCol, nodeId)) {
          ItemsTable.RemoveRow(ItemsTable.Current);
        }                
        return true;
      }
      return false;
    }

    public void CleanupUnusedTypes() {
      var usedTypeIds = ItemsTable.Rows.Values
          .Select(row => row[Cx.ItemTypeIdCol].Value.AsInt32())
          .Concat(ItemsTable.Rows.Values.Select(row => row[Cx.ItemStatusCol].Value.AsInt32()))
          .ToHashSet();

      // Don't remove system types
      usedTypeIds.Add(nullTypeId);
      usedTypeIds.Add(categoriesTypeId);
      usedTypeIds.Add(statusTypesId);
      usedTypeIds.Add(itemTypesId);

      var typesToRemove = TypesTable.Rows.Values
          .Where(row => !usedTypeIds.Contains(row.Id))
          .Select(row => row.Id)
          .ToList();

      foreach (var typeId in typesToRemove) {
        if (TypesTable.FindFirst(Cx.TypeIdCol, typeId)) {
          TypesTable.RemoveRow(TypesTable.Current);
        }          
      }
      
    }

    // Convenience methods for common scenarios
    public bool RemoveNodeAndChildren(int nodeId) =>
        RemoveNode(nodeId, RemoveStrategy.DeleteCascade);

    public bool RemoveNodeOnly(int nodeId) =>
        RemoveNode(nodeId, RemoveStrategy.ReparentToGrandparent);


    #endregion

    #region Todos 

    public Nodes MakeTodoList(string listName, string[] items) {
      var todoStatusId = TypesTable.TypesByName(Cx.StatusStart);
      var todolistId = ItemsTable.GetTodoByName(listName, todoTypeId, todoStatusId);
      if (todolistId == 0) {
        _logger.LogInformation($"Creating new todo list '{listName}'");
        todolistId = ItemsTable.AddItem(todoRootNodeId, todoTypeId, todoStatusId, 0, listName, "Todo item: " + listName, DateTime.Now, null);
      } else { 
        _logger.LogInformation($"Using existing todo list '{listName}' (Id: {todolistId})");
      }
      var todoList = GetNodesById(todolistId, maxDepth: 1);
      if (todoList == null) throw new InvalidOperationException("Failed to create or retrieve todo list node.");
      foreach (var itemName in items) {
        if (!string.IsNullOrWhiteSpace(itemName)) {
          var existingItem = todoList.Subnodes.FirstOrDefault(n => string.Equals(n.Name, itemName, StringComparison.OrdinalIgnoreCase));
          if (existingItem == null) {
            var todoItem = new Nodes {
              ParentId = todolistId,
              TypeId = todoTypeId,
              TypeName = Cx.TypeTodo,
              StatusId = todoStatusId,
              Status = Cx.StatusStart,
              Rank = todoList.Subnodes.Count + 1,
              Name = itemName.Trim(),
              Details = "",
              Created = DateTime.Now,
              Modified = DateTime.Now
            };            
            todoList.Subnodes.Add(todoItem);
          }
        }
      }      
      return AddUpdateNode(todoList);
    }

    public Nodes? GetNextTodoItem(string listName, int maxDepth = 1) { 

      _logger.LogDebug($"GetNextTodoItem: listName='{listName}', maxDepth={maxDepth}");

      var todoStatusId = TypesTable.TypesByName(Cx.StatusStart);
      var todoStatusInProgressId = TypesTable.TypesByName(Cx.StatusInProgress);      

      IEnumerable<int>? todoItems = null;
      string todoListName = listName;
      int todolistId = 0;
      if ( todoListName != "") {
        todolistId = ItemsTable.GetTodoByName(todoListName, todoTypeId, todoStatusId);
        if (todolistId == 0) {
          return null;  // no such list
        } else {
          todoItems = ItemsTable.Rows.Values
            .Where(row => row[Cx.ItemParentCol].Value.AsInt32() == todolistId &&
                          row[Cx.ItemTypeIdCol].Value.AsInt32() == todoTypeId &&
                          row[Cx.ItemStatusCol].Value.AsInt32() == todoStatusId)
            .OrderBy(row => row.Id)
            .Select(row => row.Id)
            .ToList();
          _logger.LogDebug($"Found {todoItems.Count()} todo items in list '{todoListName}'.");
        }
      }
     
      if (todoItems != null && todoItems.Count() > 0 ) { 
        foreach (var itemId in todoItems) {
          var todoItem = GetNodesById(itemId, Cx.TypeTodoMaxDepth, Cx.StatusStart, Cx.TypeTodo);
          if (todoItem != null) {
            Nodes? subItem = null;
            if (todoItem.Subnodes.Any() && maxDepth > 0) {
              foreach (var sub in todoItem.Subnodes) {            
                subItem = GetNextTodoItem(sub.Name, maxDepth - 1);  // recursive search in sublist
                if (subItem != null) break;
              }
            }
            todoItem = subItem == null ? todoItem : subItem;
            if (todoItem == null) continue;
            // Mark as In Progress unless it was marked done in a subitem
            if (subItem == null) { 
              if (ItemsTable.FindFirst(Cx.ItemIdCol, todoItem.Id)) { 
                ItemsTable.Edit();
                ItemsTable.Current[Cx.ItemStatusCol].Value = todoStatusInProgressId;
                ItemsTable.Current[Cx.ItemModifiedCol].Value = DateTime.Now;
                ItemsTable.Post();
                return GetNodesById(todoItem.Id, maxDepth: 1, Cx.StatusStart, Cx.TypeTodo);
              }
            }
            return todoItem;  // latest was recursive set in subItem.
          }
        }
      }
      int todoCount = todoItems?.Count() ?? 0;
      if ((todoItems == null || todoCount == 0) && todolistId > 0 ) {  // all sub items complete need to return the list        
        if (ItemsTable.FindFirst(Cx.ItemIdCol, todolistId)) {
          ItemsTable.Edit();
          ItemsTable.Current[Cx.ItemStatusCol].Value = todoStatusInProgressId;
          ItemsTable.Current[Cx.ItemModifiedCol].Value = DateTime.Now;
          ItemsTable.Post();
          return GetNodesById(todolistId, maxDepth: 1, Cx.StatusStart, Cx.TypeTodo);
        }
      }

      return null;
    }

    public Nodes MarkTodoDone(int itemId) { 
        var todoStatusCompleteId = TypesTable.TypesByName(Cx.StatusComplete);        
        if (ItemsTable.FindFirst(Cx.ItemIdCol, itemId)) {
          ItemsTable.Edit();
          ItemsTable.Current[Cx.ItemStatusCol].Value = todoStatusCompleteId;
          ItemsTable.Current[Cx.ItemModifiedCol].Value = DateTime.Now;
          ItemsTable.Current[Cx.ItemCompletedCol].ValueString = DateTime.Now.AsString();
          ItemsTable.Post();          
        }
        return GetNodesById(itemId, maxDepth: 1, Cx.StatusStart, Cx.TypeTodo);
    }

    public Nodes RestoreAsTodo(int itemId) {
      var todoStatusStartId = TypesTable.TypesByName(Cx.StatusStart);      
      if (ItemsTable.FindFirst(Cx.ItemIdCol, itemId)) {
        ItemsTable.Edit();
        ItemsTable.Current[Cx.ItemStatusCol].Value = todoStatusStartId;
        ItemsTable.Current[Cx.ItemModifiedCol].Value = DateTime.Now;      
        ItemsTable.Post();
      }
      return GetNodesById(itemId, maxDepth: 1, Cx.StatusStart, Cx.TypeTodo);
    }

    public Nodes MarkTodoCancel(int itemId) {
      var todoStatusCancelId = TypesTable.TypesByName(Cx.StatusCancelled);
      if (ItemsTable.FindFirst(Cx.ItemIdCol, itemId)) {
        ItemsTable.Edit();
        ItemsTable.Current[Cx.ItemStatusCol].Value = todoStatusCancelId;
        ItemsTable.Current[Cx.ItemModifiedCol].Value = DateTime.Now;
        ItemsTable.Current[Cx.ItemCompletedCol].ValueString = DateTime.Now.AsString();
        ItemsTable.Post();
      }
      return GetNodesById(itemId, maxDepth: 1, Cx.StatusStart, Cx.TypeTodo);
    }

    #endregion

  }

  public enum RemoveStrategy {
    PreventIfHasChildren,
    DeleteCascade,
    OrphanChildren,
    ReparentToGrandparent
  }


}
