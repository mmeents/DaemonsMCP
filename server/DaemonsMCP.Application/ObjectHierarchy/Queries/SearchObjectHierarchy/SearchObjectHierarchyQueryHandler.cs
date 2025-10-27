using DaemonsMCP.Application.ObjectHierarchy.Queries.SearchObjectHierarchy;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ObjectHierarchy.Queries.SearchObjectHierarchy {
  public class SearchObjectHierarchyQueryHandler 
    : IRequestHandler<SearchObjectHierarchyQuery, SearchObjectHierarchyResult> {

    private IObjectHierarchyRepository _repository;
    public SearchObjectHierarchyQueryHandler(IObjectHierarchyRepository repository ) {
      _repository = repository;
    }

    public async Task<SearchObjectHierarchyResult> Handle(
      SearchObjectHierarchyQuery request, CancellationToken cancellationToken) {

      var result = await _repository.Search(request, cancellationToken);
      return result;
    }
  }


}
