using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ForWeb.GetReadme {
  // Query
  public record GetReadmeForWebQuery() : IRequest<ReadmeWebResponse>;

  // Response DTO
  public record ReadmeWebResponse {
    public string AccessAndAuthorizationDetails { get; init; } = string.Empty;
    public ReadmeContentDto Readme { get; init; } = null!;
    public List<EndpointDescription> Endpoints { get; init; } = new();
    public List<ProjectDto> Projects { get; init; } = new();
  }

  public record ReadmeContentDto {
    public List<ItemDto> Content { get; init; } = new();
    public string Version { get; init; } = string.Empty;
    public DateTime LastUpdated { get; init; }
  }

  public record EndpointDescription(
      string Method,
      string Path,
      string Description,
      List<ParameterDescription>? Parameters = null
  );

  public record ParameterDescription(
      string Name,
      string Type,
      bool Required = false,
      string? Description = null
  );
}
