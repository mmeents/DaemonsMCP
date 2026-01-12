using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Scriban;
using Scriban.Runtime;

namespace DaemonsMCP.Application.Templates.Commands.ExecuteTemplate {

  public record ExecuteTemplateCommand(
    int TemplateId,
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
      var template = await _modelRepository.GetByIdWithPropertiesAsync(request.TemplateId, cancellationToken);

      // 2. Get target model ID from property or override
      var targetModelId = request.TargetModelId
        ?? GetTargetModelIdFromProperties(template);

      if (!targetModelId.HasValue) {
        return new TemplateExecutionResult {
          Success = false,
          ErrorMessage = "No target model specified"
        };
      }

      // 3. Load target model with full tree
      var targetModel = await _modelRepository.GetByIdWithChildrenAsync(targetModelId.Value, maxDepth: 3, cancellationToken);

      // 4. Execute template (handle children recursively)
      var outputs = await ExecuteTemplateRecursive(template, targetModel, cancellationToken);

      // 5. Get filename from properties
      var relativeFileName = template.Properties
        .FirstOrDefault(p => p.PropertyKey == "RelativeFileName")?.PropertyValue
        ?? $"{targetModel.Name}.txt";

      // Replace tokens in filename
      relativeFileName = relativeFileName.Replace("{model.name}", targetModel.Name);

      // 6. Optionally save to file system
      if (request.SaveToFile) {
        var project = await _projectRepository.GetByIdAsync(template.ProjectId, cancellationToken);
        var projectPath = Path.Combine(project.RootPath, relativeFileName);
        await File.WriteAllTextAsync(projectPath, string.Join("\n\n", outputs), cancellationToken);           
      }

      return new TemplateExecutionResult {
        Success = true,
        RenderedCode = string.Join("\n\n", outputs),
        RelativeFileName = relativeFileName,
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

    private async Task<List<string>> ExecuteTemplateRecursive(Model template, Model targetModel, CancellationToken cancellationToken) {
      var outputs = new List<string>();

      // Execute child templates first (ordered by rank)
      if (template.Children?.Any() == true) {
        foreach (var childTemplate in template.Children.OrderBy(c => c.Rank)) {
          var childOutputs = await ExecuteTemplateRecursive(childTemplate, targetModel, cancellationToken);
          outputs.AddRange(childOutputs);
        }
      }

      // Execute this template's code (if any)
      if (!string.IsNullOrEmpty(template.Code)) {
        string modelType = ((Mte)template.ModelTypeId).TemplateToModel().ToTypeString().ToLower();
        var scriptObject = new ScriptObject();
        scriptObject[modelType] = targetModel;
        //scriptObject["children_output"] = outputs; // Child template outputs

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        var scribanTemplate = Template.Parse(template.Code);
        var rendered = scribanTemplate.Render(context);

        outputs.Insert(0, rendered); // Parent output goes first
      }

      return outputs;

    }
  }
}
