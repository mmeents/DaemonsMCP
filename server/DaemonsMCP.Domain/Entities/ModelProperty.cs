using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class ModelProperty {
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string PropertyKey { get; set; } = string.Empty;
    public string? PropertyValue { get; set; }
    public int? PropertyValueTypeId { get; set; }
    public int? PropertyEditorTypeId { get; set; }

    // Navigation properties
    public Model Model { get; set; } = null!;
    public ModelType? PropertyValueType { get; set; }
    public ModelType? PropertyEditorType { get; set; }

    // EF Core constructor
    public ModelProperty() { }

    public ModelProperty(
      int modelId, 
      string propertyKey, 
      string? propertyValue = null,
      int? propertyModelTypeId = null,
      int? propertyEditorTypeId = null
    ) {
      ModelId = modelId;
      PropertyKey = propertyKey;
      PropertyValue = propertyValue;
      PropertyValueTypeId = propertyModelTypeId;
      PropertyEditorTypeId = propertyEditorTypeId;
    }

    public void Update(string? propertyValue, int? propertyValueTypeId = null, int? propertyEditorTypeId = null) {
      PropertyValue = propertyValue;
      if (propertyValueTypeId.HasValue) { 
        PropertyValueTypeId = propertyValueTypeId;
      }
      if (propertyEditorTypeId.HasValue) { 
        PropertyEditorTypeId = propertyEditorTypeId;
      }
    }
  }
}
