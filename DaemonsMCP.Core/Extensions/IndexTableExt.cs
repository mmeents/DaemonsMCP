using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using PackedTables.Net;

namespace DaemonsMCP.Core.Extensions {
  public static class IndexTableExt {

    public static TableModel MakeFilesTable(this PackedTableSet IndexTables) {      
      if (IndexTables[Cx.FileTbl] != null) {
        IndexTables.RemoveTable(Cx.FileTbl);
      }
      var Files = IndexTables.AddTable(Cx.FileTbl);
      Files.AddColumn(Cx.FilePathNameCol, ColumnType.String);
      Files.AddColumn(Cx.FileSizeCol, ColumnType.Int64);
      Files.AddColumn(Cx.FileModifiedCol, ColumnType.DateTime);
      return Files;
    }

    public static TableModel MakeClassesTable(this PackedTableSet IndexTables) {      
      if (IndexTables[Cx.ClassesTbl] != null) {
        IndexTables.RemoveTable(Cx.ClassesTbl);
      }
      var Classes = IndexTables.AddTable(Cx.ClassesTbl);
      Classes.AddColumn(Cx.ClassesFileIdCol, ColumnType.Int32); 
      Classes.AddColumn(Cx.ClassesNameCol, ColumnType.String);
      Classes.AddColumn(Cx.ClassesFileNameCol, ColumnType.String);
      Classes.AddColumn(Cx.ClassesNamespaceCol, ColumnType.String);
      Classes.AddColumn(Cx.ClassesLineStartCol, ColumnType.Int32);
      Classes.AddColumn(Cx.ClassesLineEndCol, ColumnType.Int32);
      return Classes;
    }

    public static TableModel MakeMethodsTable(this PackedTableSet IndexTables) {      
      if (IndexTables[Cx.MethodsTbl] != null) {
        IndexTables.RemoveTable(Cx.MethodsTbl);
      }
      var Methods = IndexTables.AddTable(Cx.MethodsTbl);
      Methods.AddColumn(Cx.MethodsClassIdCol, ColumnType.Int32); // foreign key to Files table
      Methods.AddColumn(Cx.MethodsNameCol, ColumnType.String);
      Methods.AddColumn(Cx.MethodsReturnTypeCol, ColumnType.String);
      Methods.AddColumn(Cx.MethodsParametersCol, ColumnType.String);
      Methods.AddColumn(Cx.MethodsLineStartCol, ColumnType.Int32);
      Methods.AddColumn(Cx.MethodsLineEndCol, ColumnType.Int32);
      return Methods;
    }

    public static TableModel MakePropertiesTable(this PackedTableSet IndexTables) {      
      if (IndexTables[Cx.PropertiesTbl] != null) {
        IndexTables.RemoveTable(Cx.PropertiesTbl);
      }
      var Properties = IndexTables.AddTable(Cx.PropertiesTbl);
      Properties.AddColumn(Cx.PropertiesClassIdCol, ColumnType.Int32);
      Properties.AddColumn(Cx.PropertiesNameCol, ColumnType.String);
      Properties.AddColumn(Cx.PropertiesTypeCol, ColumnType.String);
      Properties.AddColumn(Cx.PropertiesLineStartCol, ColumnType.Int32);
      Properties.AddColumn(Cx.PropertiesLineEndCol, ColumnType.Int32);
      Properties.AddColumn(Cx.PropertiesHasGetterCol, ColumnType.Boolean);
      Properties.AddColumn(Cx.PropertiesHasSetterCol, ColumnType.Boolean);
      return Properties;
    }

    public static TableModel MakeEventsTable(this PackedTableSet IndexTables) {      
      if (IndexTables[Cx.EventsTbl] != null) {
        IndexTables.RemoveTable(Cx.EventsTbl);
      }
      var Events = IndexTables.AddTable(Cx.EventsTbl);
      Events.AddColumn(Cx.EventsClassIdCol, ColumnType.Int32);
      Events.AddColumn(Cx.EventsNameCol, ColumnType.String);
      Events.AddColumn(Cx.EventsTypeCol, ColumnType.String);
      Events.AddColumn(Cx.EventsLineStartCol, ColumnType.Int32);
      Events.AddColumn(Cx.EventsLineEndCol, ColumnType.Int32);
      return Events;
    }


    public static List<IndexFileItem> RemoveFileRange(this List<IndexFileItem> fileItems, HashSet<string> files) { 
        var toRemove = fileItems.Where( f => files.Contains(  f.FilePathName));
        foreach( var rem in toRemove ) {
          fileItems.Remove(rem);
        }
        return fileItems;
    }

    public static List<IndexClassItem> Subtract(this List<IndexClassItem> classItems, IndexClassItem minusItem) {         
      classItems.RemoveAll( c => c.Name == minusItem.Name && c.Namespace == minusItem.Namespace); 
      return classItems;
    }

    public static List<IndexMethodItem> Subtract(this List<IndexMethodItem> methodItems, IndexMethodItem minusItem) {         
      methodItems.RemoveAll( m => m.Name == minusItem.Name && m.ClassId == minusItem.ClassId); 
      return methodItems;
    }

  }
}
