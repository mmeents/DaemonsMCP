using DaemonsMCP.Core.Models;
using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Extensions {
  public static class StorageTableExt {

    public static int TypesByName(this TableModel typeTbl, string typeName) {      
       if (typeTbl == null) throw new ArgumentNullException(nameof(typeTbl));
       if( typeTbl.FindFirst(Cx.TypeNameCol, typeName)) { 
            return typeTbl.Current.Id;
       }       
       return 0;
    }

    public static TableModel MakeItemsTable(this PackedTableSet StorageTables) {      
      if (StorageTables[Cx.ItemsTbl] != null) {
        StorageTables.RemoveTable(Cx.ItemsTbl);
      }
      var ItemsTbl = StorageTables.AddTable(Cx.ItemsTbl);
      ItemsTbl.AddColumn(Cx.ItemParentCol, ColumnType.Int32); // foreign key to Types table
      ItemsTbl.AddColumn(Cx.ItemTypeIdCol, ColumnType.Int32);
      ItemsTbl.AddColumn(Cx.ItemStatusCol, ColumnType.Int32);
      ItemsTbl.AddColumn(Cx.ItemRankCol, ColumnType.Int32);
      ItemsTbl.AddColumn(Cx.ItemNameCol, ColumnType.String);
      var acol = ItemsTbl.AddColumn(Cx.ItemDetailsCol, ColumnType.String);
      acol.IsRequired = false; // disable validation.
      ItemsTbl.AddColumn(Cx.ItemCreatedCol, ColumnType.DateTime);
      ItemsTbl.AddColumn(Cx.ItemModifiedCol, ColumnType.DateTime);
      acol = ItemsTbl.AddColumn(Cx.ItemCompletedCol, ColumnType.DateTime);
      acol.IsRequired = false; // disable validation.

      return ItemsTbl;
    }

    public static int AddType(this TableModel TypesTable, int parentId, int typeEnum, string typeName, string description) {
      if (TypesTable == null) throw new ArgumentNullException(nameof(TypesTable));
      var newRow = TypesTable.AddRow();      
      int id = newRow.Id;
      newRow[Cx.TypeParentCol].Value = parentId;
      newRow[Cx.TypeEnumCol].Value = typeEnum;
      newRow[Cx.TypeNameCol].Value = typeName;
      newRow[Cx.TypeDescriptionCol].ValueString = description;
      TypesTable.Post();      
      return id;
    }

    public static int AddItem(this TableModel ItemsTable, int parentId, int typeId, int statusId, int rank, 
      string name, string details, DateTime? created = null, DateTime? completed = null) {
      if (ItemsTable == null) throw new ArgumentNullException(nameof(ItemsTable));
      var newRow = ItemsTable.AddRow();      
      int id = newRow.Id;
      newRow[Cx.ItemParentCol].Value = parentId;
      newRow[Cx.ItemTypeIdCol].Value = typeId;
      newRow[Cx.ItemStatusCol].Value = statusId;
      newRow[Cx.ItemRankCol].Value = rank;
      newRow[Cx.ItemNameCol].ValueString = name;
      newRow[Cx.ItemDetailsCol].ValueString = details;
      if (!created.HasValue) {
        created = DateTime.Now;
      }
      newRow[Cx.ItemCreatedCol].Value = created;
      newRow[Cx.ItemModifiedCol].Value = DateTime.Now;
      if (completed.HasValue) {
        newRow[Cx.ItemCompletedCol].Value = completed.Value;
      }
      ItemsTable.Post();      
      return id;
    }

  }
}
