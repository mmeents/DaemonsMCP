using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;

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
        (int)Mte.SqlDateTimeType => (int)Mte.CSharpDateTimeType,   // DateTime
        (int)Mte.SqlDateType => (int)Mte.CSharpDateType,           // Date
        (int)Mte.SqlTimeType => (int)Mte.CSharpTimeType,           // Time
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
        Mte.RootTemplate => Mte.FolderTemplate,
        Mte.FolderTemplate => Mte.FolderTemplate,
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

    public static bool IsActive(this Model model) { 
      var activeProperty = model.Properties.FirstOrDefault(p => p.PropertyKey == "IsActive");
      if (activeProperty == null) { 
        return false;
      }
      var isActive = ((activeProperty?.PropertyValue ?? "0" ) == "1");
      return isActive;
    }

    public static string GetDefaultModelTemplate(this Mte modelType) {
      return modelType switch {
        Mte.TableModel => @"{{ has_index = 0; index_column = ''; size = ''; type=''; null_value = ''; ident_str = ''; }}{{
func as_sql_type (type_id, size) 
  case type_id
    when 101 
      'BIT'
    when 102 
      'SMALLINT'
    when 103 
      'INT'
    when 104 
      'BIGINT'
    when 112 
      if size != ''; 'DECIMAL' + size; else; 'DECIMAL(18,2)'; end
    when 108 
      if size != ''; 'NVARCHAR' + size; else; 'NVARCHAR(MAX)'; end
    when 110 
      if size != ''; 'VARCHAR' + size; else; 'VARCHAR(MAX)'; end
    when 106 
      'uniqueidentifier'
    when 114 
      'DATETIME2' + size
    when 118 
      'DATE'
    when 120 
      'TIME'
    else 
      'NVARCHAR(MAX)'
  end  
end   
}}
Create Table {{table.name}} ({{ for column in table.children }}
  {{ column.name }}{{ for prop in column.properties; 
    if prop.property_key == 'ColumnType' }}{{ type = prop.property_value; }}{{ end }}{{
    if prop.property_key == 'IsNullable' && prop.property_value == '1'}}{{ null_value='NULL'}}{{ else if prop.property_key == 'IsNullable' && prop.property_value == '0'}}{{ null_value='NOT NULL';}}{{ end }}{{ 
    if prop.property_key == 'IsPrimaryKey' && prop.property_value == '1' }}{{ ident_str = 'IDENTITY(1,1) ';}}{{ has_index = 1; index_column = column.name; }}{{ end }}{{
    if prop.property_key == 'MaxLength'}}{{ size = prop.property_value; end; }}{{
}}{{ end }} {{ as_sql_type type size }} {{ ident_str }}{{ null_value}}{{ ident_str = ''; }},{{ end }}{{ if has_index == 1 }}  
  CONSTRAINT [pk_{{ table.name | string.remove '.'}}_{{ index_column | string.remove '.'}}] PRIMARY KEY CLUSTERED ({{ index_column }}){{ end }}
)

",
        Mte.ViewTemplate => "",
        Mte.FunctionTemplate => "",
        Mte.ProcedureTemplate => "",
        Mte.InterfaceTemplate => "",
        Mte.ControllerTemplate => "",
        Mte.ClassTemplate => "",
        _ => ""
      };
    }
  }
}
