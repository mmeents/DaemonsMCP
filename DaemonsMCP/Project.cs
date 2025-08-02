using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP
{

    // This class represents a collection of projects and their details.
    // it will need to be updated by the user to match their local environment.
    // Add by hand, Create a new project and add it to the Projects dictionary.
    public static class Nx
    {
        public static readonly string BasePath = "C:\\";        

        public static readonly string Project1Path = Path.Combine(BasePath, "GithubMM\\DaemonsMCP");
        public static readonly string Project2Path = Path.Combine(BasePath, "GithubMM\\PackedTables.NET");
        public static readonly string Project3Path = Path.Combine(BasePath, "GithubMM\\PackedTables.Tabs");

        public static readonly Project project1 = new Project(
            "DaemonsMCP", 
            "This MCP Service Project.", 
            Project1Path);

        public static readonly Project project2 = new Project(
            "PackedTables.NET", 
            "This is project that saves table as a string", 
            Project2Path);

        public static readonly Project project3 = new Project(
            "PackedTables.Tabs",
            "This Windows Forms UI Class Library",
            Project1Path);

    public static IReadOnlyDictionary<string, Project> Projects { get; } = new Dictionary<string, Project>
        {
            { project1.Name, project1 },
            { project2.Name, project2 },
            { project3.Name, project3 }
        };

    }

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
