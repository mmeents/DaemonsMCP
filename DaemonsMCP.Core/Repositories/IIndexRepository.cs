using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {

  public interface IIndexRepository {
    public ProjectIndexModel? GetProjectIndex(string projectName);
    public Task<List<ClassListing>> GetClassListingsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null);
    public Task<ClassContent> GetClassContentAsync(string projectName, int classId);
    public Task<OperationResult> AddUpdateClassContentAsync(string projectName, ClassContent classContent);

    public Task<List<MethodListing>> GetMethodListingsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter = null);

    public Task<MethodContent> GetMethodContentAsync(string projectName, int methodId);
    public Task<OperationResult> AddUpdateMethodAsync(string projectName, MethodContent methodContent);


  }
}
