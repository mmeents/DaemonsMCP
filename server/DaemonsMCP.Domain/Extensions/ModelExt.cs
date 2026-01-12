using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Enums;

namespace DaemonsMCP.Domain.Extensions {
  public static class ModelExt {

    public static int GetCSharpTypeFromSqlModelType(int sqlModelTypeId) {
      return sqlModelTypeId switch {
        (int)Mte.SqlBitType => (int)Mte.CSharpBoolType,          // bool
        (int)Mte.SqlSmallIntType => (int)Mte.CSharpShortType,     // short
        (int)Mte.SqlIntType => (int)Mte.CSharpIntType,          // int
        (int)Mte.SqlBigIntType => (int)Mte.CSharpLongType,       // long
        (int)Mte.SqlDecimalType => (int)Mte.CSharpDecimalType,      // decimal
        (int)Mte.SqlVarcharType => (int)Mte.CSharpStringType,      // string
        (int)Mte.SqlNVarcharType => (int)Mte.CSharpStringType,     // string
        (int)Mte.SqlUniqueIdentifierType => (int)Mte.CSharpStringType, // string
    //    (int)Mte.SqlDateTimeType => (int)Mte.CSharpDateTime,   // DateTime
    //    (int)Mte.SqlDateType => (int)Mte.CSharpDateType,           // Date
    //    (int)Mte.SqlTimeType => (int)Mte.CSharpTimeType,           // Time
        _ => (int)Mte.CSharpStringType  // default to string
      };
    }

    public static string ToTypeString(this Mte modelType) { 
      return modelType switch {
        Mte.EditorList => "Editor List",
        Mte.HttpMethod => "HTTP Methods",
        Mte.CSharpAccessModifiers => "C# Access Modifiers",

        Mte.SqlTypes => "SQL Types",
        Mte.SqlBitType => "bit",
        Mte.SqlSmallIntType => "smallint",
        Mte.SqlIntType => "int",
        Mte.SqlBigIntType => "bigint",
        Mte.SqlDecimalType => "decimal",
        Mte.SqlVarcharType => "varchar",
        Mte.SqlNVarcharType => "nvarchar",
        Mte.SqlUniqueIdentifierType => "string",
        Mte.SqlDateTimeType => "DateTime",
        Mte.SqlDateType => "DateTime",
        Mte.SqlTimeType => "DateTime",

        Mte.CSharpTypes => "C# Types",
        Mte.CSharpClassType => "class",
        Mte.CSharpRecordType => "record",
        Mte.CSharpStructType => "struct",
        Mte.CSharpStringType => "string",
        Mte.CSharpBoolType => "bool",
        Mte.CSharpCharType => "char",
        Mte.CSharpIntType => "int",
        Mte.CSharpLongType => "long",
        Mte.CSharpShortType => "short",
        Mte.CSharpDecimalType => "decimal",
        Mte.CSharpDoubleType => "double",
        Mte.CSharpFloatType => "float",
        Mte.CSharpByteType => "byte",
        Mte.CSharpDateTimeType => "DateTime",
        Mte.CSharpDateType => "DateTime",
        Mte.CSharpTimeType => "DateTime",

        Mte.ProjectModel => "Project",
        Mte.DatabaseModel => "Database",
        Mte.TablesModel => "Tables",
        Mte.TableModel => "Table",
        Mte.TableColumnModel => "Column",

        Mte.ViewsModel => "Views",
        Mte.ViewModel => "View",
        Mte.ViewColumnModel => "Column",
        Mte.FunctionsModel => "Functions",
        Mte.FunctionModel => "Function",
        Mte.FunctionParameterModel => "Parameter",
        Mte.ProceduresModel => "Procedures",
        Mte.ProcedureModel => "Procedure",
        Mte.ProcedureParameterModel => "Parameter",
        Mte.ApiModel => "API",
        Mte.InterfaceModel => "Interface",
        Mte.InterfacePropertyModel => "Property",
        Mte.InterfaceMethodModel => "Method",
        Mte.InterfaceMethodParameterModel => "Parameter",
        Mte.ControllerModel => "Controller",
        Mte.ControllerPropertyModel => "Property",
        Mte.ControllerMethodModel => "Method",
        Mte.ControllerMethodParameterModel => "Parameter",
        Mte.ClassModel => "Class",
        Mte.ClassPropertyModel => "Property",
        Mte.ClassMethodModel => "Method",
        Mte.ClassMethodParameterModel => "Parameter",

        Mte.RootTemplate => "RootFolderTemplate",
        Mte.FolderTemplate => "FolderTemplate",

        Mte.DatabaseTemplate => "DatabaseTemplate",
        Mte.ApiTemplate => "APITemplate",

        Mte.TablesTemplate => "TablesTemplate",
        Mte.ViewsTemplate => "ViewsTemplate",
        Mte.FunctionsTemplate => "FunctionsTemplate",
        Mte.ProceduresTemplate => "ProceduresTemplate",

        Mte.TableTemplate => "TableTemplate",
        Mte.ViewTemplate => "ViewTemplate",
        Mte.FunctionTemplate => "FunctionTemplate",
        Mte.ProcedureTemplate => "ProcedureTemplate",
        
        Mte.InterfaceTemplate => "InterfaceTemplate",
        Mte.ControllerTemplate => "ControllerTemplate",
        Mte.ClassTemplate => "ClassTemplate",

        _ => "Unknown Model Type"
      };
    }
    /*
{{func as_csharp }}
  {{ case $0
    when 101 "bool"
    when 102 "short"
    when 103 "int"
    when 104 "long"
    when 112 "decimal"
    when 108 "string"
    when 110 "string"
    when 106 "string"
    when 114 "DateTime"
    when 118 "DateTime"
    when 120 "DateTime"
    else "string" 
  end}}
{{end}}
     */
    public static string GetCSharpFromSqlModelType(int sqlModelTypeId) {
      return sqlModelTypeId switch {
        (int)Mte.SqlBitType => "bool",
        (int)Mte.SqlSmallIntType => "short",
        (int)Mte.SqlIntType => "int",
        (int)Mte.SqlBigIntType => "long",
        (int)Mte.SqlDecimalType => "decimal",
        (int)Mte.SqlVarcharType => "string",
        (int)Mte.SqlNVarcharType => "string",
        (int)Mte.SqlUniqueIdentifierType => "string",
        (int)Mte.SqlDateTimeType => "DateTime",
        (int)Mte.SqlDateType => "DateTime",
        (int)Mte.SqlTimeType => "DateTime",
        _ => "string"  // default to string
      };
    }

    public static Mte TemplateToModel(this Mte model) { 
      return model switch {
        Mte.DatabaseTemplate => Mte.DatabaseModel,
        Mte.ApiTemplate => Mte.ApiModel,
        Mte.TablesTemplate => Mte.TablesModel,
        Mte.ViewsTemplate => Mte.ViewsModel,
        Mte.FunctionsTemplate => Mte.FunctionsModel,
        Mte.ProceduresTemplate => Mte.ProceduresModel,
        Mte.TableTemplate => Mte.TableModel,
        Mte.ViewTemplate => Mte.ViewModel,
        Mte.FunctionTemplate => Mte.FunctionModel,
        Mte.ProcedureTemplate => Mte.ProcedureModel,
        Mte.InterfaceTemplate => Mte.InterfaceModel,
        Mte.ControllerTemplate => Mte.ControllerModel,
        Mte.ClassTemplate => Mte.ClassModel,
        _ => model
      };
    }

  }
}
