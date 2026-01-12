using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;

namespace DaemonsMCP.Domain.Enums {

  // Model Type Enum
  public enum Mte {
    TypeRoot = 1,
    CategoryRoot = 2,
    EditorList = 3,

    HiddenEditor = 10,
    BooleanEditor = 11,
    IntegerEditor = 12,
    StringEditor = 13,
    FilenameEditor = 14,
    DateEditor = 15,
    TimeEditor = 16,
    DecimalEditor = 17,
    PasswordEditor = 18,
    LookupTypeEditor = 19,
    LookupModelEditor = 20,

    HttpMethod = 50,
    HttpGetMethod = 52,
    HttpPostMethod = 53,
    HttpPutMethod = 54,
    HttpDeleteMethod = 55,
    HttpPatchMethod =56,

    CSharpAccessModifiers = 60,
    CSharpPublicModifier = 62,
    CSharpPrivateModifier = 64,
    CSharpProtectedModifier = 66,
    CSharpInternalModifier = 68,

    SqlTypes = 100,
    SqlBitType = 101,
    SqlSmallIntType = 102,
    SqlIntType = 103,
    SqlBigIntType = 104,    
    SqlUniqueIdentifierType = 106,
    SqlVarcharType = 108,
    SqlNVarcharType = 110,
    SqlDecimalType = 112,
    SqlDateTimeType = 114,
    SqlDateType = 118,
    SqlTimeType = 120,

    CSharpTypes = 150,
    CSharpClassType = 152,
    CSharpRecordType = 154,
    CSharpStructType = 156,
    CSharpStringType = 158,
    CSharpBoolType = 160,
    CSharpCharType = 162,
    CSharpIntType = 164,
    CSharpLongType = 166,
    CSharpShortType = 168,
    CSharpDecimalType = 170,
    CSharpDoubleType = 172,
    CSharpFloatType = 174,
    CSharpByteType = 176,
    CSharpDateTimeType = 178,
    CSharpDateType = 180,
    CSharpTimeType = 182,

    ProjectModel = 200,
    DatabaseModel = 210,
    TablesModel = 220,
    TableModel = 222,
    TableColumnModel = 226,

    ViewsModel = 230,
    ViewModel = 232,
    ViewColumnModel = 236,

    FunctionsModel = 240,
    FunctionModel = 242,
    FunctionParameterModel = 246,

    ProceduresModel = 250,
    ProcedureModel = 252,
    ProcedureParameterModel = 256,

    ApiModel = 300,
    InterfaceModel = 301,
    InterfacePropertyModel = 302,
    InterfaceMethodModel = 304,
    InterfaceMethodParameterModel = 306,
    ControllerModel = 310,
    ControllerPropertyModel = 312,
    ControllerMethodModel = 314,
    ControllerMethodParameterModel = 316,
    ClassModel = 320,
    ClassPropertyModel = 322,
    ClassMethodModel = 324,
    ClassMethodParameterModel = 326,


    RootTemplate = 400,           // Organizer, shows in project context menu
    FolderTemplate = 402,         // Organizes templates, can wrap children output

    DatabaseTemplate = 410,       // Operates on Database root (210)
    ApiTemplate = 412,            // Operates on Api folder (300)

    TablesTemplate = 420,         // Operates on Tables folder (220)
    ViewsTemplate = 422,          // Operates on Views folder (230)
    FunctionsTemplate = 424,      // Operates on Functions folder (240)
    ProceduresTemplate = 426,     // Operates on Procedures folder (250)
                                  // 
    TableTemplate = 430,          // Operates on one Table (222)
    ViewTemplate = 432,           // Operates on one View (232)
    FunctionTemplate = 434,       // Operates on one Function (242)
    ProcedureTemplate = 436,      // Operates on one Procedure (252)
    
    InterfaceTemplate = 440,      // Operates on one Interface (301)
    ControllerTemplate = 442,     // Operates on one Controller (310)
    ClassTemplate = 444,          // Operates on one Class (320)
   
    
  }

  public static class ModelPropExt {

    public static List<ModelPropertyDto> GetDefaultPropertiesByModelId(int modelTypeId) {
      if (DefaultProps.ContainsKey((Mte)modelTypeId)) {
        // Return a deep copy of each property
        return DefaultProps[(Mte)modelTypeId]
          .Select(p => new ModelPropertyDto {
            PropertyKey = p.PropertyKey,
            PropertyValue = p.PropertyValue,
            PropertyValueTypeId = p.PropertyValueTypeId
          })
          .ToList();
      }
      return new List<ModelPropertyDto>();
    }

    private static Dictionary<Mte, List<ModelPropertyDto>> DefaultProps = new() {
      #region Model Templates
      {
        Mte.DatabaseModel, 
        new List<ModelPropertyDto>(){ 
            new ModelPropertyDto { PropertyKey = "ConnectionString", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType }
        } 
      }, // 210 DatabaseModel  
      {
        Mte.TablesModel,
        new List<ModelPropertyDto>()
      }, // 220 TablesModel
      {
        Mte.TableModel,
        new List<ModelPropertyDto> {
            new ModelPropertyDto { PropertyKey = "Schema", PropertyValue = "dbo", PropertyValueTypeId=(int)Mte.SqlVarcharType },
            new ModelPropertyDto { PropertyKey = "DisplayName", PropertyValue = "", PropertyValueTypeId=(int)Mte.SqlVarcharType }
          }
      }, // 222 TableModel
      { 
        Mte.TableColumnModel, 
        new List<ModelPropertyDto> {            
            new ModelPropertyDto { PropertyKey = "ColumnType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.SqlTypes },
            new ModelPropertyDto { PropertyKey = "IsPrimaryKey", PropertyValue = "0", PropertyValueTypeId=(int)Mte.SqlBitType },
            new ModelPropertyDto { PropertyKey = "IsIndexed", PropertyValue = "0", PropertyValueTypeId=(int)Mte.SqlBitType },
            new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
            new ModelPropertyDto { PropertyKey = "MaxLength", PropertyValue = "100", PropertyValueTypeId=(int)Mte.SqlIntType },            
        }
      }, // 226 TableColumnModel
      {
          Mte.ViewsModel,
          new List<ModelPropertyDto>()
      }, // 230 ViewsModel
      {
          Mte.ViewModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "Schema", PropertyValue = "dbo", PropertyValueTypeId=(int)Mte.SqlVarcharType },
              new ModelPropertyDto { PropertyKey = "DisplayName", PropertyValue = "", PropertyValueTypeId=(int)Mte.SqlVarcharType }
          }
      }, // 232 ViewModel
      {
          Mte.ViewColumnModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "ColumnType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.SqlTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
              new ModelPropertyDto { PropertyKey = "MaxLength", PropertyValue = "100", PropertyValueTypeId=(int)Mte.SqlIntType },
          }
      }, // 236 ViewColumnModel
      {
          Mte.FunctionsModel,
          new List<ModelPropertyDto>()
      }, // 240 FunctionsModel
      {
          Mte.FunctionModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "Schema", PropertyValue = "dbo", PropertyValueTypeId=(int)Mte.SqlVarcharType },
              new ModelPropertyDto { PropertyKey = "DisplayName", PropertyValue = "", PropertyValueTypeId=(int)Mte.SqlVarcharType }
          }
      }, // 242 FunctionModel
      {
          Mte.FunctionParameterModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "ParameterType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.SqlTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
              new ModelPropertyDto { PropertyKey = "MaxLength", PropertyValue = "100", PropertyValueTypeId=(int)Mte.SqlIntType },
          }
      }, // 246 FunctionParameterModel
      {
          Mte.ProceduresModel,
          new List<ModelPropertyDto>()
      }, // 250 ProceduresModel
      {
          Mte.ProcedureModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "Schema", PropertyValue = "dbo", PropertyValueTypeId=(int)Mte.SqlVarcharType },
              new ModelPropertyDto { PropertyKey = "DisplayName", PropertyValue = "", PropertyValueTypeId=(int)Mte.SqlVarcharType }
          }
      }, // 252 ProcedureModel
      {
          Mte.ProcedureParameterModel,
          new List<ModelPropertyDto> {
              new ModelPropertyDto { PropertyKey = "ParameterType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.SqlTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
              new ModelPropertyDto { PropertyKey = "MaxLength", PropertyValue = "100", PropertyValueTypeId=(int)Mte.SqlIntType },
          }
      }, // 256 ProcedureParameterModel

      { Mte.ApiModel, 
        new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "Namespace", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },           
          }
      }, // 300 ApiModel
      { Mte.InterfaceModel, 
        new List<ModelPropertyDto>() {              
              new ModelPropertyDto { PropertyKey = "BaseType", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
        }
      }, // 301 InterfaceModel
      { Mte.InterfacePropertyModel, 
        new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "PropertyType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
          }
      }, // 302 InterfacePropertyModel
      { Mte.InterfaceMethodModel, 
        new List<ModelPropertyDto>() {              
              new ModelPropertyDto { PropertyKey = "ReturnType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },                            
          }
      }, // 304 InterfaceMethodModel
      { Mte.InterfaceMethodParameterModel, 
        new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "ParameterType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },              
          }
      }, // 306 InterfaceMethodParameterModel
      { Mte.ControllerModel, 
        new List<ModelPropertyDto>()  {
              new ModelPropertyDto { PropertyKey = "Namespace", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
              new ModelPropertyDto { PropertyKey = "BaseType", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
              new ModelPropertyDto { PropertyKey = "Interface", PropertyValue = "", PropertyValueTypeId=(int)Mte.InterfaceModel },
          }
      }, // 310 ControllerModel
      { Mte.ControllerPropertyModel, 
          new List<ModelPropertyDto>(){
              new ModelPropertyDto { PropertyKey = "PropertyType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
          }
      }, // 312 ControllerPropertyModel
      { Mte.ControllerMethodModel, 
          new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "HttpMethod", PropertyValue = "52", PropertyValueTypeId=(int)Mte.HttpMethod },
              new ModelPropertyDto { PropertyKey = "AccessModifier", PropertyValue = "62", PropertyValueTypeId=(int)Mte.CSharpAccessModifiers },
              new ModelPropertyDto { PropertyKey = "ReturnType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsAsync", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsVirtual", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsStatic", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsAbstract", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsSealed", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
          }
      }, // 314 ControllerMethodModel
      { Mte.ControllerMethodParameterModel, 
          new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "ParameterType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },              
          }
      }, // 316 ControllerMethodParameterModel
      { Mte.ClassModel, 
        new List<ModelPropertyDto>()  {
              new ModelPropertyDto { PropertyKey = "Namespace", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
              new ModelPropertyDto { PropertyKey = "BaseType", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
              new ModelPropertyDto { PropertyKey = "Interface", PropertyValue = "", PropertyValueTypeId=(int)Mte.InterfaceModel },
          }
      }, // 320 ClassModel
      { Mte.ClassPropertyModel, 
          new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "PropertyType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },              
          }
      }, // 322 ClassPropertyModel
      { Mte.ClassMethodModel, 
          new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "AccessModifier", PropertyValue = "62", PropertyValueTypeId=(int)Mte.CSharpAccessModifiers },
              new ModelPropertyDto { PropertyKey = "ReturnType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsAsync", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsVirtual", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsStatic", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsAbstract", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
              new ModelPropertyDto { PropertyKey = "IsSealed", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },
          }
      }, // 324 ClassMethodModel
      { Mte.ClassMethodParameterModel, 
          new List<ModelPropertyDto>() {
              new ModelPropertyDto { PropertyKey = "ParameterType", PropertyValue = "103", PropertyValueTypeId=(int)Mte.CSharpTypes },
              new ModelPropertyDto { PropertyKey = "IsNullable", PropertyValue = "1", PropertyValueTypeId=(int)Mte.CSharpBoolType },              
          }
      }, // 326 ClassMethodParameterModel
      #endregion
      #region Templates
      {
        Mte.RootTemplate,        
        new List<ModelPropertyDto>(){ 
           new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
            new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
            new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 400 RootTemplate
      { 
        Mte.FolderTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        } 
      }, // 402 FolderTemplate
      { Mte.DatabaseTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Database", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },          
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }          
        }
      }, // 410 DatabaseTemplate
      {Mte.ApiTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Api", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 412 ApiTemplate
      { Mte.TablesTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Tables", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 420 TablesTemplate
      { Mte.ViewsTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Views", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 422 ViewsTemplate      
      { Mte.FunctionsTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Functions", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 424 FunctionsTemplate
      { Mte.ProceduresTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Procedures", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 426 ProceduresTemplate
      {
        Mte.TableTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Table", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 430 TableTemplate
      {
        Mte.ViewTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "View", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 432 ViewTemplate
      {
          Mte.FunctionTemplate,
          new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Function", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
          }
      }, // 434 FunctionTemplate
      {
          Mte.ProcedureTemplate,
          new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Procedure", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
          }
      }, // 436 ProcedureTemplate
      { Mte.InterfaceTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Interface", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 440 InterfaceTemplate
      { Mte.ControllerTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Controller", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      }, // 442 ControllerMethodModel
      { Mte.ClassTemplate,
        new List<ModelPropertyDto>() {
          new ModelPropertyDto { PropertyKey = "IsActive", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType },
          new ModelPropertyDto { PropertyKey = "Class", PropertyValue = "", PropertyValueTypeId=(int)Mte.LookupModelEditor },
          new ModelPropertyDto { PropertyKey = "RelativeFileName", PropertyValue = "", PropertyValueTypeId=(int)Mte.CSharpStringType },
          new ModelPropertyDto { PropertyKey = "OverwriteExistingFiles", PropertyValue = "1", PropertyValueTypeId=(int)Mte.SqlBitType }
        }
      } // 444 ClassTemplate
      #endregion 
    };



  }
}
