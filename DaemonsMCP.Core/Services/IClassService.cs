using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {

  public interface IClassService {
    public Task<OperationResult> GetClassesAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null);

    public Task<OperationResult> GetClassContentAsync(string projectName, int classID);

    public Task<OperationResult> AddUpdateClassContentAsync(string projectName, ClassContent classContent);

    public Task<OperationResult> GetMethodsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter = null);

    public Task<OperationResult> GetMethodContentAsync(string projectName, int methodID);

    public Task<OperationResult> AddUpdateMethodAsync(string projectName, MethodContent methodContent);

  }

}
