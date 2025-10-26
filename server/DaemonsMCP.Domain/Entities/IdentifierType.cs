using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities;
  public class IdentifierType {
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;  
    
    public IdentifierType() { }

    public static IdentifierType Create(string name) {
      return new IdentifierType {
        Name = name
      };
    }

    public void UpdateName(string name) {
      Name = name;
    }
  }
