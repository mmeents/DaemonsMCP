using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace DaemonsMCP
{
    public static class Px { 
        public const string listProjects = "local-list-projects";
        public const string ProjectDescription  = "Gets list of available projects. A project is the name of the root folder.";

        public const string listProjectDirectory = "local-list-project-directories";
        public const string ProjectDirectoryDescription = "Gets list of directories in the project. The project name is required and must match the current project.";

        public const string listProjectFiles = "local-list-project-files";
        public const string ProjectFilesDescription = "Gets list of files in the project. The project name is required and must match the current project.";

        public const string getProjectFile = "local-get-project-file";
        public const string GetProjectFileDescription = "Gets the content of a file in the project. The project name is required and must match the current project. The path to the file is also required.";

        public const string projectNameParam = "projectName";
        public const string projectNameParamDesc = "Project name from list-projects";

        public const string pathParam = "path";
        public const string pathParamDesc = "Path to the directory or file. If empty, the root of the project is used.";

        public const string filterParam = "filter";
        public const string filterParamDesc = "Filter for files or directories. If empty, no filter is applied.";
    }
  
}
