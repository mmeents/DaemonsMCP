using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Application.Items.Commands.DeleteItem {
  public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand,bool> {
    private readonly IItemRepository _repository;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(IItemRepository repository, ILogger<DeleteItemCommandHandler> logger) {
      _repository = repository;
      _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken) {
      var strategy = request.Strategy;
      var item = await _repository.GetByIdAsync(request.ItemId);
      if (item == null) {
        _logger.LogWarning("Attempted to delete item with ID {ItemId} but it was not found", request.ItemId);
        throw new Exception("Item not found");
      }
      if (strategy == DeleteStrategy.PreventIfHasChildren && item.Children.Any()) {
        _logger.LogWarning("Attempted to delete item with ID {ItemId} using PreventIfHasChildren strategy but it has children", request.ItemId);
        throw new Exception("Strategy prevents deletion of item with children");
      }
      if (strategy == DeleteStrategy.ReparentToGrandparent && item.Children.Any()) {
        _logger.LogInformation("Reparenting children of item with ID {ItemId} to grandparent", request.ItemId);
        foreach (var child in item.Children) {
          child.MoveTo(item.ParentId);
        }
      }
      if (strategy == DeleteStrategy.OrphanChildren && item.Children.Any()) {
        _logger.LogInformation("Orphaning children of item with ID {ItemId}", request.ItemId);
        foreach (var child in item.Children) {
          child.MoveTo(null);
        }
      }
      await _repository.DeleteAsync(item, cancellationToken);
      await _repository.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("Item with ID {ItemId} deleted using strategy {Strategy}", request.ItemId, strategy);
      return true;
    }
  }

}
