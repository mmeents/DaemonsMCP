using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Scriban;
using Scriban.Runtime;

namespace DaemonsMCP.Application.Templates.Commands.ExecuteTemplate {

  public record ExecuteTemplateCommand(
    int TemplateModelId,
    int? TargetModelId = null,  // Optional override
    bool SaveToFile = false      // Preview vs Execute
  ) : IRequest<TemplateExecutionResult>;

  public record TemplateExecutionResult {
    public string RenderedCode { get; set; }
    public string RelativeFileName { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> GeneratedFiles { get; set; } // For composite templates
  }

  internal class ExecuteTemplateCommandHandler(
    IModelRepository modelRepository, 
    IProjectRepository projectRepository
  ) : IRequestHandler<ExecuteTemplateCommand, TemplateExecutionResult> {
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IModelRepository _modelRepository = modelRepository;
    public async Task<TemplateExecutionResult> Handle(ExecuteTemplateCommand request, CancellationToken cancellationToken) {
      // 1. Load template with properties
      var template = await _modelRepository.GetByIdWithChildrenAsync(request.TemplateModelId, maxDepth: 3, cancellationToken);

      // 2. Get target model ID from property or override
      var targetModelId = request.TargetModelId
        ?? GetTargetModelIdFromProperties(template) ?? template?.Id;

      if (!targetModelId.HasValue) {
        return new TemplateExecutionResult {
          Success = false,
          ErrorMessage = "No target model specified"
        };
      }
           

      // 4. Execute template (handle children recursively)
      var outputs = await ExecuteTemplateRecursive(template, cancellationToken);

      // 5. Get filename from properties
      var project = await _projectRepository.GetByIdAsync(template.ProjectId, cancellationToken);      
      var relativeFileName = template.Properties
        .FirstOrDefault(p => p.PropertyKey == "RelativeFileName")?.PropertyValue
        ?? $"{template.Name}.txt";
      var projectPath = Path.GetFullPath( Path.Combine(project.RootPath, relativeFileName));

      // 6. Optionally save to file system
      if (request.SaveToFile) {        
        await File.WriteAllTextAsync(projectPath, string.Join("\n\n", outputs), cancellationToken);           
      }

      return new TemplateExecutionResult {
        Success = true,
        RenderedCode = string.Join("\n\n", outputs),
        RelativeFileName = projectPath,  //relativeFileName,
        GeneratedFiles = request.SaveToFile ? new List<string> { relativeFileName } : new()
      };
    }

    private int? GetTargetModelIdFromProperties(Model template) {

      string modelType = ((Mte)template.ModelTypeId).TemplateToModel().ToTypeString();
      var targetModelProp = template.Properties.FirstOrDefault(p => p.PropertyKey == modelType);
      if (targetModelProp != null && int.TryParse(targetModelProp.PropertyValue, out int targetModelId)) {
        return targetModelId;
      }
      return null;
    }

    private async Task<List<string>> ExecuteTemplateRecursive(Model template,  CancellationToken cancellationToken) {
      try {
        var outputs = new List<string>();
        if (template == null) { 
            return outputs;
        }
        bool isTableModel = ((Mte)template.ModelTypeId) == Mte.TableModel;
        bool isTemplate = ((Mte)template.ModelTypeId).IsTemplate();

        if (isTemplate) {
          if (!template.IsActive()) {
            return outputs;
          }
        } else if (!isTableModel) {
          return outputs;
        }       

        var targetModelId = GetTargetModelIdFromProperties(template) ?? template.Id;

        var targetModel = await _modelRepository.GetByIdWithChildrenAsync(targetModelId, maxDepth: 3, cancellationToken);

        // Execute child templates first (ordered by rank)
        if (template.Children?.Any() == true) {
          foreach (var childTemplate in template.Children.OrderBy(c => c.Rank)) {
            var childOutputs = await ExecuteTemplateRecursive(childTemplate, cancellationToken);
            outputs.AddRange(childOutputs);
          }
        }

        var scriptToExecute = "";
        if ( template.ModelTypeId == (int)Mte.TableModel) {  // ((Mte)template.ModelTypeId).IsModel() &&
          scriptToExecute = ((Mte)template.ModelTypeId).GetDefaultModelTemplate();
        } else if (((Mte)template.ModelTypeId).IsTemplate()) {
          if (!string.IsNullOrEmpty(template.Code)) {
            scriptToExecute = template.Code;
          } else { 
            scriptToExecute = ((Mte)template.ModelTypeId).GetDefaultModelTemplate();
          }
        }


        // Execute this template's code (if any)
        if (!string.IsNullOrEmpty(scriptToExecute)) {
          string modelType = ((Mte)template.ModelTypeId).TemplateToModel().ToTypeString().ToLower();
          var scriptObject = new ScriptObject();
          scriptObject[modelType] = targetModel;
          //scriptObject["children_output"] = outputs; // Child template outputs

          var context = new TemplateContext();
          context.PushGlobal(scriptObject);

          var scribanTemplate = Template.Parse(scriptToExecute);
          var rendered = scribanTemplate.Render(context);

          outputs.Insert(0, rendered); // Parent output goes first
        }

        return outputs;

      } catch (Exception ex) { 
        return new List<string> { $"Error executing template {template.Id}: {ex.Message}" };

      }
    }

  }

}
