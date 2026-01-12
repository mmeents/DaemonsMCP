import { Injectable } from '@angular/core';

export enum Mte {
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
  CSharpAccessModifiers = 60,
  
  SqlTypes = 100,
  SqlBitType = 101,
  SqlSmallIntType = 102,
  SqlIntType = 103,
  SqlBigIntType = 104,
  SqlUniqueIdentifierType = 106,
  SqlVarCharType = 108,
  SqlNVarCharType = 110,
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
  
  RootTemplate = 400,
  FolderTemplate = 402,
  DatabaseTemplate = 410,
  ApiTemplate = 412,
  TablesTemplate = 420,
  ViewsTemplate = 422,
  FunctionsTemplate = 424,
  ProceduresTemplate = 426,
  TableTemplate = 430,
  ViewTemplate = 432,
  FunctionTemplate = 434,
  ProcedureTemplate = 436,
  InterfaceTemplate = 440,
  ControllerTemplate = 442,
  ClassTemplate = 444,
}

@Injectable({
  providedIn: 'root'
})
export class ModelTypeService {
  // Expose enum for template use
  readonly Mte = Mte;

  /**
   * Get the appropriate editor type for a PropertyValueTypeId
   */
  getEditorType(propertyValueTypeId: number | null | undefined): number {
    if (!propertyValueTypeId) return Mte.StringEditor;

    // Direct editor types (10-20 range)
    if (propertyValueTypeId >= 10 && propertyValueTypeId < 50) {
      return propertyValueTypeId;
    }

    // SQL Types → map to editors
    const sqlTypeEditors: Record<number, number> = {
      [Mte.SqlBitType]: Mte.BooleanEditor,
      [Mte.SqlSmallIntType]: Mte.IntegerEditor,
      [Mte.SqlIntType]: Mte.IntegerEditor,
      [Mte.SqlBigIntType]: Mte.IntegerEditor,
      [Mte.SqlDecimalType]: Mte.DecimalEditor,
      [Mte.SqlVarCharType]: Mte.StringEditor,
      [Mte.SqlNVarCharType]: Mte.StringEditor,
      [Mte.SqlUniqueIdentifierType]: Mte.StringEditor,
      [Mte.SqlDateTimeType]: Mte.DateEditor,
      [Mte.SqlDateType]: Mte.DateEditor,
      [Mte.SqlTimeType]: Mte.TimeEditor,
    };

    // C# Types → map to editors
    const csharpTypeEditors: Record<number, number> = {
      [Mte.CSharpBoolType]: Mte.BooleanEditor,
      [Mte.CSharpIntType]: Mte.IntegerEditor,
      [Mte.CSharpLongType]: Mte.IntegerEditor,
      [Mte.CSharpShortType]: Mte.IntegerEditor,
      [Mte.CSharpDecimalType]: Mte.DecimalEditor,
      [Mte.CSharpDoubleType]: Mte.DecimalEditor,
      [Mte.CSharpFloatType]: Mte.DecimalEditor,
      [Mte.CSharpStringType]: Mte.StringEditor,
      [Mte.CSharpCharType]: Mte.StringEditor,
      [Mte.CSharpByteType]: Mte.IntegerEditor,
      [Mte.CSharpDateTimeType]: Mte.DateEditor,
      [Mte.CSharpDateType]: Mte.DateEditor,
      [Mte.CSharpTimeType]: Mte.TimeEditor,      
    };

    const TypePickerEditors: Record<number, number> = {
      [Mte.EditorList]: Mte.LookupTypeEditor,
      [Mte.HttpMethod]: Mte.LookupTypeEditor,
      [Mte.CSharpAccessModifiers]: Mte.LookupTypeEditor,
      [Mte.SqlTypes]: Mte.LookupTypeEditor,
      [Mte.CSharpTypes]: Mte.LookupTypeEditor,
    };

    const ModelPickerEditors: Record<number, number> = {
      [Mte.DatabaseModel]: Mte.LookupModelEditor,
      [Mte.TableModel]: Mte.LookupModelEditor,
      [Mte.ViewModel]: Mte.LookupModelEditor,
      [Mte.FunctionModel]: Mte.LookupModelEditor,
      [Mte.ProcedureModel]: Mte.LookupModelEditor,
      [Mte.ApiModel]: Mte.LookupModelEditor,
      [Mte.InterfaceModel]: Mte.LookupModelEditor,
      [Mte.ControllerModel]: Mte.LookupModelEditor,
      [Mte.ClassModel]: Mte.LookupModelEditor,
      [Mte.DatabaseTemplate]: Mte.LookupModelEditor,
      [Mte.ApiTemplate]: Mte.LookupModelEditor,
      [Mte.TablesTemplate]: Mte.LookupModelEditor,
      [Mte.ViewsTemplate]: Mte.LookupModelEditor,
      [Mte.FunctionsTemplate]: Mte.LookupModelEditor,
      [Mte.ProceduresTemplate]: Mte.LookupModelEditor,
      [Mte.TableTemplate]: Mte.LookupModelEditor,
      [Mte.ViewTemplate]: Mte.LookupModelEditor,
      [Mte.FunctionTemplate]: Mte.LookupModelEditor,
      [Mte.ProcedureTemplate]: Mte.LookupModelEditor,
      [Mte.InterfaceTemplate]: Mte.LookupModelEditor,
      [Mte.ControllerTemplate]: Mte.LookupModelEditor,
      [Mte.ClassTemplate]: Mte.LookupModelEditor,
    };

    return sqlTypeEditors[propertyValueTypeId] 
      || csharpTypeEditors[propertyValueTypeId] 
      || TypePickerEditors[propertyValueTypeId]
      || ModelPickerEditors[propertyValueTypeId]
      || Mte.StringEditor;
  }

  /**
   * Check if a PropertyValueTypeId is a reference type (shows dropdown of children)
   */
  isReferenceType(propertyValueTypeId: number | null | undefined): boolean {
    if (!propertyValueTypeId) return false;
    
    // Categories that have children to select from
    const categoryTypes = [
      Mte.EditorList,              // 3
      Mte.HttpMethod,              // 50
      Mte.CSharpAccessModifiers,   // 60
      Mte.SqlTypes,                // 100
      Mte.CSharpTypes,             // 150
    ];
    
    return categoryTypes.includes(propertyValueTypeId);
  }

  /**
   * Check if a PropertyValueTypeId is a model reference (LookupModelEditor)
   */
  isModelReference(propertyValueTypeId: number | null | undefined): boolean {
    return propertyValueTypeId === Mte.LookupModelEditor;
  }

  /**
   * Get target model type for a template type
   */
  getTargetModelType(templateTypeId: number): number | null {
    const mapping: Record<number, number> = {
      // Collection templates
      [Mte.DatabaseModel] : Mte.DatabaseModel,  // DatabaseModel → Database
      [Mte.TablesModel]: Mte.TablesModel,  // TablesModel → Tables
      [Mte.TableModel]: Mte.TableModel,  // TableModel → Table      
      [Mte.ViewsModel]: Mte.ViewsModel,  // ViewsModel → Views
      [Mte.ViewModel]: Mte.ViewModel,  // ViewModel → View
      [Mte.FunctionsModel]: Mte.FunctionsModel,  // FunctionsModel → Functions
      [Mte.FunctionModel]: Mte.FunctionModel,  // FunctionModel → Function
      [Mte.ProceduresModel]: Mte.ProceduresModel,  // ProceduresModel → Procedures
      [Mte.ProcedureModel]: Mte.ProcedureModel,  // ProcedureModel → Procedure

      [Mte.ApiModel]: Mte.ApiModel,  // ApiModel → Api
      [Mte.InterfaceModel]: Mte.InterfaceModel,  // InterfaceModel → Interface
      [Mte.InterfacePropertyModel]: Mte.InterfacePropertyModel,  // InterfacePropertyModel → InterfaceProperty
      [Mte.InterfaceMethodModel]: Mte.InterfaceMethodModel,  // InterfaceMethodModel → InterfaceMethod
      [Mte.ControllerModel]: Mte.ControllerModel,  // ControllerModel → Controller
      [Mte.ControllerPropertyModel]: Mte.ControllerPropertyModel,  // ControllerPropertyModel → ControllerProperty
      [Mte.ControllerMethodModel]: Mte.ControllerMethodModel,  // ControllerMethodModel → ControllerMethod
      [Mte.ClassModel]: Mte.ClassModel,  // ClassModel → Class
      [Mte.ClassPropertyModel]: Mte.ClassPropertyModel,  // ClassPropertyModel → ClassProperty
      [Mte.ClassMethodModel]: Mte.ClassMethodModel,  // ClassMethodModel → ClassMethod

      [Mte.DatabaseTemplate]: Mte.DatabaseModel,  // DatabaseTemplate → Database
      [Mte.ApiTemplate]: Mte.ApiModel,  // ApiTemplate → Api
      [Mte.TablesTemplate]: Mte.TablesModel,  // TablesTemplate → Tables 
      [Mte.ViewsTemplate]: Mte.ViewsModel,  // ViewsTemplate → Views
      [Mte.FunctionsTemplate]: Mte.FunctionsModel,  // FunctionsTemplate → Function
      [Mte.ProceduresTemplate]: Mte.ProceduresModel,  // ProceduresTemplate → Procedure
      [Mte.TableTemplate]: Mte.TableModel,  // TableTemplate → Table
      [Mte.ViewTemplate]: Mte.ViewModel,  // ViewTemplate → View
      [Mte.FunctionTemplate]: Mte.FunctionModel,  // FunctionTemplate → Function
      [Mte.ProcedureTemplate]: Mte.ProcedureModel,  // ProcedureTemplate → Procedure

      [Mte.InterfaceTemplate]: Mte.InterfaceModel,  // InterfaceTemplate → Interface
      [Mte.ControllerTemplate]: Mte.ControllerModel,  // ControllerTemplate → Controller
      [Mte.ClassTemplate]: Mte.ClassModel,  // ClassTemplate → Class
    };
    
    return mapping[templateTypeId] || null;
  }


   protected getTargetTypeIdForTemplate(templateTypeId: number): number | null {
    const mapping: Record<number, number> = {
      [Mte.DatabaseModel]: Mte.DatabaseModel,  // DatabaseModel → Database
      [Mte.TablesModel]: Mte.TablesModel,  // TablesModel → Tables
      [Mte.TableModel]: Mte.TableModel,  // TableModel → Table      
      [Mte.ViewsModel]: Mte.ViewsModel,  // ViewsModel → Views
      [Mte.ViewModel]: Mte.ViewModel,  // ViewModel → View
      [Mte.FunctionsModel]: Mte.FunctionsModel,  // FunctionsModel → Functions
      [Mte.FunctionModel]: Mte.FunctionModel,  // FunctionModel → Function
      [Mte.ProceduresModel]: Mte.ProceduresModel,  // ProceduresModel → Procedures
      [Mte.ProcedureModel]: Mte.ProcedureModel,  // ProcedureModel → Procedure

      [Mte.ApiModel]: Mte.ApiModel,  // ApiModel → Api
      [Mte.InterfaceModel]: Mte.InterfaceModel,  // InterfaceModel → Interface
      [Mte.InterfacePropertyModel]: Mte.InterfacePropertyModel,  // InterfacePropertyModel → InterfaceProperty
      [Mte.InterfaceMethodModel]: Mte.InterfaceMethodModel,  // InterfaceMethodModel → InterfaceMethod
      [Mte.ControllerModel]: Mte.ControllerModel,  // ControllerModel → Controller
      [Mte.ControllerPropertyModel]: Mte.ControllerPropertyModel,  // ControllerPropertyModel → ControllerProperty
      [Mte.ControllerMethodModel]: Mte.ControllerMethodModel,  // ControllerMethodModel → ControllerMethod
      [Mte.ClassModel]: Mte.ClassModel,  // ClassModel → Class
      [Mte.ClassPropertyModel]: Mte.ClassPropertyModel,  // ClassPropertyModel → ClassProperty
      [Mte.ClassMethodModel]: Mte.ClassMethodModel,  // ClassMethodModel → ClassMethod

      [Mte.DatabaseTemplate]: Mte.DatabaseModel,  // DatabaseTemplate → Database
      [Mte.ApiTemplate]: Mte.ApiModel,  // ApiTemplate → Api
      [Mte.TablesTemplate]: Mte.TablesModel,  // TablesTemplate → Tables 
      [Mte.ViewTemplate]: Mte.ViewModel,  // ViewTemplate → View
      [Mte.FunctionsTemplate]: Mte.FunctionsModel,  // FunctionsTemplate → Function
      [Mte.ProceduresTemplate]: Mte.ProceduresModel,  // ProceduresTemplate → Procedure
      [Mte.TableTemplate]: Mte.TableModel,  // TableTemplate → Table      
      [Mte.FunctionTemplate]: Mte.FunctionModel,  // FunctionTemplate → Function
      [Mte.ProcedureTemplate]: Mte.ProcedureModel,  // ProcedureTemplate → Procedure
      [Mte.InterfaceTemplate]: Mte.InterfaceModel,  // InterfaceTemplate → Interface
            
    };
    return mapping[templateTypeId] || null;
  }

  /**
   * Check if a model type is a template type
   */
  isTemplateType(modelTypeId: number): boolean {
    return modelTypeId >= 400 && modelTypeId < 500;
  }
}