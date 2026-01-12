using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Models;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.xUnit.Tests.Scriban {
  public class TemplateTests {

    [Fact]
    public void TestMVP() {

      string templateText = @"Table {{myass.name}}:{{ for column in myass.columns }}
  {{ column.name }}:{{ column.column_type }}{{ end }}";
      tableSchema myClass = new tableSchema {
        Name = "Actors",
        Columns = new List<columnSchema>
        {
          new columnSchema { Name = "Actor1", ColumnType = "TypeA" },
          new columnSchema { Name = "Actor2", ColumnType = "TypeB" },
          new columnSchema { Name = "Actor3", ColumnType = "TypeC" }
        }
      };

      var scriptObject1 = new ScriptObject();
      scriptObject1["myass"] = myClass;

      var context = new TemplateContext();
      context.PushGlobal(scriptObject1);

      var template = Template.Parse(templateText);
      var result = template.Render(context);
      Console.WriteLine(result);
      Assert.True(1 == 1);
    }

    private ModelDto CreateTestTableModel() { 
      var model = new ModelDto(){
        Id = 1,
        ParentId = 6,             // some random number at the moment.
        ProjectId = 10,           // some random project number  
        Rank = 1,
        Name = "TestTable",
        ModelTypeId = (int)Mte.TableModel,
        ModelTypeName = "Table",
        Properties = ModelPropExt.GetDefaultPropertiesByModelId((int)Mte.TableModel),
        Children = new List<ModelDto>()
      };

      model.Children.Add(new ModelDto()
      {
        Id = 2,
        ParentId = 1,
        ProjectId = 10,
        Rank = 1,
        Name = "Id",
        ModelTypeId = (int)Mte.TableColumnModel,
        ModelTypeName = "Table Column",
        Properties = ModelPropExt.GetDefaultPropertiesByModelId((int)Mte.TableColumnModel),
        Children = new List<ModelDto>()
      });      

      model.Children.Add(new ModelDto()
      {
        Id = 3,
        ParentId = 1,
        ProjectId = 10,
        Rank = 2,
        Name = "Name",
        ModelTypeId = (int)Mte.TableColumnModel,
        ModelTypeName = "Table Column",
        Properties = ModelPropExt.GetDefaultPropertiesByModelId((int)Mte.TableColumnModel),
        Children = new List<ModelDto>()
      });

      model.Children.Add(new ModelDto()
      {
        Id = 4,
        ParentId = 1,
        ProjectId = 10,
        Rank = 3,
        Name = "Value",
        ModelTypeId = (int)Mte.TableColumnModel,
        ModelTypeName = "Table Column",
        Properties = ModelPropExt.GetDefaultPropertiesByModelId((int)Mte.TableColumnModel),
        Children = new List<ModelDto>()
      });

      foreach( ModelPropertyDto prop in model.Properties) {
        prop.ModelId = model.Id;
      }
      foreach( ModelDto m in model.Children) {
        foreach (var item in m.Properties) {
          item.ModelId = m.Id;
        }
      }

      model.Children[0].Properties.First(p => p.PropertyKey == "IsPrimaryKey").PropertyValue = "1";
      model.Children[0].Properties.First(p => p.PropertyKey == "IsNullable").PropertyValue = "0";
      model.Children[1].Properties.First(p => p.PropertyKey == "ColumnType").PropertyValue = "nvarchar(100)";
      model.Children[1].Properties.First(p => p.PropertyKey == "IsNullable").PropertyValue = "0";
      model.Children[1].Properties.First(p => p.PropertyKey == "ColumnType").PropertyValueTypeId = (int)Mte.SqlVarcharType;
      model.Children[1].Properties.First(p => p.PropertyKey == "ColumnType").PropertyValue = "nvarchar(100)";
      model.Children[2].Properties.First(p => p.PropertyKey == "ColumnType").PropertyValue = "nvarchar(100)";
      model.Children[2].Properties.First(p => p.PropertyKey == "ColumnType").PropertyValueTypeId = (int)Mte.SqlVarcharType;

      return model;
    }

    [Fact]
    public void TestMVP2() {

      ModelDto model = CreateTestTableModel();


      string templateText = @"{{ has_index = 0; index_column = ''; }}{{
func as_sql_type (type_id) 
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
      'DECIMAL(18,2)'
    when 108 
      'NVARCHAR(MAX)'
    when 110 
      'VARCHAR(MAX)'
    when 106 
      'uniqueidentifier'
    when 114 
      'DATETIME'
    when 118 
      'DATE'
    when 120 
      'TIME'
    else 
      'NVARCHAR(MAX)'
  end  
end   
}}
Create Table {{model.name}} ({{ for column in model.children }}
  {{ column.name }}{{ for prop in column.properties; 
    if prop.property_key == 'ColumnType' }} {{ as_sql_type prop.property_value }}{{ end }}{{
    if prop.property_key == 'IsNullable' && prop.property_value == '1' }} NULL{{ else if prop.property_key == 'IsNullable' && prop.property_value == '0' }} NOT NULL{{ end }}{{ 
    if prop.property_key == 'IsPrimaryKey' && prop.property_value == '1' }} IDENTITY(1,1){{ has_index = 1; index_column = column.name; }}{{ end }}{{ end }}{{ end }}{{ if has_index == 1 }}  
  CONSTRAINT [pk_{{ model.name | string.remove '.'}}_{{ index_column | string.remove '.'}}] PRIMARY KEY CLUSTERED ({{ index_column }}){{ end }}
)";

      var scriptObject1 = new ScriptObject();
      scriptObject1["model"] = model;

      var context = new TemplateContext();
      context.PushGlobal(scriptObject1);

      var template = Template.Parse(templateText);
      var result = template.Render(context);
      Console.WriteLine(result);

      Console.WriteLine($"Model DTO: {model}");
      

      Assert.True(1 == 1);
    }

    [Fact]
    public void TestMVP3() {

      ModelDto model = CreateTestTableModel();


      string templateText = @"{{ 
prop_nullable = '0'; 
func as_csharp (type_id) 
  case type_id
    when 101 
      'bool'
    when 102 
      'short'
    when 103 
      'int'
    when 104 
      'long'
    when 112 
      'decimal'
    when 108 
      'string'
    when 110 
      'string'
    when 106 
      'string'
    when 114 
      'DateTime'
    when 118 
      'DateTime'
    when 120 
      'DateTime'
    else 
      'string'
  end  
end }}
public class {{model.name}} { {{ for column in model.children; }}{{ for prop in column.properties; 
    if prop.property_key == 'ColumnType' }}{{capture column_type}}{{ as_csharp prop.property_model_type_id }}{{ end }}{{ end }}{{
    if prop.property_key == 'IsNullable' && prop.property_value == '1' }} {{ prop_nullable = '1';}}{{ end }}{{ 
  end }}
  public {{ column_type }}{{ if prop_nullable == '1' }}?{{ end }} {{ column.name }} { get; set; }{{ end }}
}";

      var scriptObject1 = new ScriptObject();
      scriptObject1["model"] = model;

      var context = new TemplateContext();
      context.PushGlobal(scriptObject1);

      var template = Template.Parse(templateText);
      if (template.HasErrors) {
        foreach (var error in template.Messages) {
          Console.WriteLine(error.Message);
        }
      } else {
        var result = template.Render(context);
        Console.WriteLine(result);
      }     

      Console.WriteLine($"Model DTO: {model}");


      Assert.True(1 == 1);
    }





  }

  public class tableSchema {
    public string Name { get; set; }
    public List<columnSchema> Columns { get; set; }
  }

  public class columnSchema {
    public string Name { get; set; }
    public string ColumnType { get; set; }
  }
}
