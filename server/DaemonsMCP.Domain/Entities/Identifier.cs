using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities;
public class Identifier {
  public int Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public Identifier() { }
  public static Identifier Create(string name) {
    return new Identifier {
      Name = name
    };
  }
  public void UpdateName(string name) {
    Name = name;
  }
}

