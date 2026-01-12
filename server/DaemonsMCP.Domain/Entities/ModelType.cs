using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class ModelType {    
    public int Id { get; set; } = 0;
    public int? OwnerTypeId { get; set; }
    public int? CategoryTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public int TypeRank { get; set; } = 0;
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsVisible { get; set; } = false;
    public bool IsReadonly { get; set; } = false;
    public string IconName { get; set; } = "";

    // Navigation properties could be added here if needed
    public ModelType? Owner { get; private set; } = null;    
    public ModelType? Category { get; private set; } = null;
    public ModelType? Editor { get; private set; } = null;
    public ICollection<ModelType> Children { get; private set; } = [];

    public ModelType() { }

  }

}
