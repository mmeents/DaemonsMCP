using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP
{   
    public class Project
    {
        public Project() { }
        public Project(string Name, string Description, string FullPath) { 
            this.Name = Name ?? throw new ArgumentNullException(nameof(Name));
            this.Description = Description ?? throw new ArgumentNullException(nameof(Description));
            this.Path = FullPath ?? throw new ArgumentNullException(nameof(FullPath));
        }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Path { get; set; } = "";
    }
}
