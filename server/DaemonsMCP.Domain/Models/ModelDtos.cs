using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Models {
  public record ModelDto {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int? ParentId { get; set; } = null;
    public int ModelTypeId { get; set; }
    public string ModelTypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string? Code { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<ModelPropertyDto> Properties { get; set; } = new();
    public List<ModelDto> Children { get; set; } = new();
  }

  public record ModelPropertyDto {
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string PropertyKey { get; set; } = string.Empty;
    public string? PropertyValue { get; set; }
    public int? PropertyValueTypeId { get; set; }
    public int? PropertyEditorTypeId { get; set; }    
    public string? PropertyValueTypeName { get; set; }
    public string? PropertyEditorTypeName { get; set; }
  }

  public record ModelTypeDto {
    public int Id { get; set; }
    public int? OwnerTypeId { get; set; }
    public int? CategoryTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public int TypeRank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public bool IsReadonly { get; set; }
    public string IconName { get; set; } = string.Empty;
    public List<ModelTypeDto> Children { get; set; } = new();
  }

  public static class  ModelDtoExt {

    public static ModelDto ToDto(this ModelDto model) => 
      new ModelDto {
        Id = model.Id,
        ProjectId = model.ProjectId,
        ParentId = model.ParentId,
        ModelTypeId = model.ModelTypeId,
        ModelTypeName = model.ModelTypeName ?? string.Empty,
        Name = model.Name,
        Rank = model.Rank,
        Code = model.Code,
        CreatedDate = model.CreatedDate,
        ModifiedDate = model.ModifiedDate,
        Properties = model.Properties?.Select(p => p.ToDto()).ToList() ?? new(),
        Children = model.Children?.Select(c => c.ToDto()).ToList() ?? new()
      };

      public static ModelDto ToDto(this Model model) =>
      new ModelDto {
        Id = model.Id,
        ProjectId = model.ProjectId,
        ParentId = model.ParentId,
        ModelTypeId = model.ModelTypeId,
        ModelTypeName = model.ModelType?.Name ?? string.Empty,
        Name = model.Name,
        Rank = model.Rank,
        Code = model.Code,
        CreatedDate = model.CreatedDate,
        ModifiedDate = model.ModifiedDate,
        Properties = model.Properties?.Select(p => p.ToDto()).ToList() ?? new(),
        Children = model.Children?.Select(c => c.ToDto()).ToList() ?? new()
      };

      public static ModelTypeDto ToDto(this ModelType modelType) =>
      new ModelTypeDto {
        Id = modelType.Id,
        OwnerTypeId = modelType.OwnerTypeId,
        CategoryTypeId = modelType.CategoryTypeId,
        EditorTypeId = modelType.EditorTypeId,
        TypeRank = modelType.TypeRank,
        Name = modelType.Name,
        Description = modelType.Description,
        IsVisible = modelType.IsVisible,
        IsReadonly = modelType.IsReadonly,
        IconName = modelType.IconName,
        Children = modelType.Children?.Select(c => c.ToDto()).ToList() ?? new()
      };

    public static ModelPropertyDto ToDto(this ModelPropertyDto modelProperty) =>
      new ModelPropertyDto {
        Id = modelProperty.Id,
        ModelId = modelProperty.ModelId,
        PropertyKey = modelProperty.PropertyKey,
        PropertyValue = modelProperty.PropertyValue,
        PropertyValueTypeId = modelProperty.PropertyValueTypeId,
        PropertyValueTypeName = modelProperty.PropertyValueTypeName,
        PropertyEditorTypeId = modelProperty.PropertyEditorTypeId,
        PropertyEditorTypeName = modelProperty.PropertyEditorTypeName
      };

    public static ModelPropertyDto ToDto(this ModelProperty modelProperty) =>
      new ModelPropertyDto {
        Id = modelProperty.Id,
        ModelId = modelProperty.ModelId,
        PropertyKey = modelProperty.PropertyKey,
        PropertyValue = modelProperty.PropertyValue,
        PropertyValueTypeId = modelProperty.PropertyValueTypeId,
        PropertyValueTypeName = modelProperty.PropertyValueType?.Name,
        PropertyEditorTypeId = modelProperty.PropertyEditorTypeId,
        PropertyEditorTypeName = modelProperty.PropertyEditorType?.Name
      };

  }
}
