using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Application.Templates.Commands.ExecuteTemplate;
using MCPSharp;
using System.ComponentModel;

namespace DaemonsMCP.Infrastructure.Tools {
  public class ModelTools {
    private static IModelToolsHandler GetTool() => DIServiceBridge.GetService<IModelToolsHandler>();

    [McpTool( Cx.SearchModelsCmd, Cx.SearchModelsCmdDesc )]
    public static async Task<string> SearchModels(
      [Description( Cx.SearchModelProjectIdParamDesc )] int projectId,
      [Description( Cx.SearchModelParentIdParamDesc)] int? parentId = null,
      [Description( Cx.SearchModelTypeIdParamDesc)] int? modelTypeId = null,
      [Description( Cx.SearchModelNameFilterParamDesc )] string? nameFilter = null,
      [Description( Cx.SearchModelMaxDepthParamDesc )] int maxDepth = 1,
      [Description( Cx.SearchModelIncludePropertiesParamDesc )] bool includeProperties = false) {
      return await GetTool().SearchModelsAsync( projectId, parentId, modelTypeId, nameFilter, maxDepth, includeProperties);
    }

    [McpTool( Cx.GetModelCmd, Cx.GetModelCmdDesc )]
    public static async Task<string> GetModelById(
      [Description( Cx.GetModelParamDesc )] int modelId,
      [Description( Cx.GetModelMaxDepthParamDesc )] int maxDepth = 1,
      [Description( Cx.GetModelIncludePropertiesParamDesc )] bool includeProperties = false) {
      return await GetTool().GetModelByIdAsync(modelId, maxDepth, includeProperties);
    }

    [McpTool( Cx.AddUpdateModelCmd, Cx.AddUpdateModelCmdDesc )]
    public static async Task<string> AddUpdateModel(
      [Description( Cx.AddUpdateModelIdParamDesc )] int id,
      [Description( Cx.AddUpdateModelProjectIdParamDesc )] int projectId,
      [Description( Cx.AddUpdateModelParentIdParamDesc )] int? parentId,
      [Description( Cx.AddUpdateModelTypeIdParamDesc )] int modelTypeId,
      [Description( Cx.AddUpdateModelNameParamDesc )] string name,
      [Description( Cx.AddUpdateModelRankParamDesc )] int rank,
      [Description( Cx.AddUpdateModelCodeParamDesc )] string? code = null) {
      return await GetTool().AddUpdateModelAsync( id, projectId, parentId, modelTypeId, name, rank, code);
    }

    [McpTool(Cx.AddUpdateModelPropertyCmd, Cx.AddUpdateModelPropertyCmdDesc)]
    public static async Task<string> AddUpdateModelProperty(
      [Description(Cx.AddUpdateModelPropertyIdParamDesc)] int Id,
      [Description(Cx.AddUpdateModelPropertyModelIdParamDesc)] int modelId,
      [Description(Cx.AddUpdateModelPropertyKeyParamDesc)] string propertyKey,
      [Description(Cx.AddUpdateModelPropertyValueParamDesc)] string propertyValue,
      [Description(Cx.AddUpdateModelPropertyValueTypeIdParamDesc)] int propertyValueTypeId) {
      return await GetTool().AddUpdateModelPropertyAsync(Id, modelId, propertyKey, propertyValue, propertyValueTypeId);
    }

    [McpTool( Cx.DeleteModelCmd, Cx.DeleteModelCmdDesc )]
    public static async Task<string> DeleteModel(
      [Description( Cx.DeleteModelIdParamDesc )] int modelId) {
      return await GetTool().DeleteModelAsync(modelId);
    }

    [McpTool(Cx.DeleteModelPropertyCmd, Cx.DeleteModelPropertyCmdDesc)]
    public static async Task<string> DeleteModelProperty(
      [Description(Cx.DeleteModelPropertyIdParamDesc)] int id) {
      return await GetTool().DeleteModelPropertyAsync(id);
    }

    [McpTool(Cx.ExecuteTemplateCmd, Cx.ExecuteTemplateCmdDesc)]
    public static async Task<string> ExecuteTemplate(
        [Description(Cx.ExecuteTemplateTemplateModelIdParamDesc)] int templateModelId,
        [Description(Cx.ExecuteTemplateTargetModelIdParamDesc)] int? targetModelId = null,
        [Description(Cx.ExecuteTemplateSaveToFileParamDesc)] bool saveToFile = false
    ) { 
      var command = new ExecuteTemplateCommand(
        templateModelId,
        targetModelId,
        saveToFile
      );
      return await GetTool().ExecuteTemplateAsync(command);
    }
  }
}
