using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Models;
using ValidationContext = DaemonsMCP.Core.Models.ValidationContext;

namespace DaemonsMCP.Core.Services {
  public interface IValidationService {
    string BuildAndValidatePath(ProjectModel project, string relativePath, bool isDirectory = false);

    public ValidationContext ValidateAndPrepare(string projectName, string path, bool ItemIsDir = false);

    public void ValidateProjectName(string projectName);

    public void ValidatePath(string path);

    public void ValidateContent(string content);

    public void ValidatePrepToSave(string path, string fullPath, string content, bool overwrite);

    public void ValidateClassContent(ClassContent content);
  }

}
