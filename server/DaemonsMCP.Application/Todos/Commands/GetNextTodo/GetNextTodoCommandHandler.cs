using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Todos.Commands.GetNextTodo {

  public class GetNextTodoCommand : IRequest<ItemDto?> {
    public GetNextTodoCommand(int? itemId = null) {
        ItemId = itemId;
    }
    public int? ItemId { get; }
  }

  public class GetNextTodoCommandHandler(IItemRepository itemRepository) : IRequestHandler<GetNextTodoCommand, ItemDto?> {
    private readonly IItemRepository _itemRepository = itemRepository;
    
    public async Task<ItemDto?> Handle(GetNextTodoCommand request, CancellationToken cancellationToken) {
      Item? item = null;
      if (request.ItemId.HasValue) {
        item = await _itemRepository.GetByIdWithChildrenAsync(request.ItemId.Value, 0, cancellationToken);
      } else {
        var list = await _itemRepository.SearchAsync(null, null, Cx.ItemTypeIdTodo, (int)Cx.StatusTypeIdNotStarted, (int)Cx.TodoRootItemId,  cancellationToken);
        item = list.FirstOrDefault();
      }

      if (item == null) {
        return null;
      }

      item = await GetNextTodoItemRecursive(item.Id, cancellationToken);
      if (item != null) {
        item.UpdateStatus(Cx.StatusTypeIdInProgress);
        await _itemRepository.UpdateAsync(item, cancellationToken);
        await _itemRepository.SaveChangesAsync(cancellationToken);
      }
      return item == null ? null : await _itemRepository.MapToDto(item, 2, cancellationToken);
    }

    private async Task<Item?> GetNextTodoItemRecursive(int itemId, CancellationToken cancellationToken) {
      Item? item = await _itemRepository.GetByIdWithChildrenAsync(itemId, 2, cancellationToken);
      if (item == null || item.Children == null || item.Children.Count == 0) {
        return item;
      }
      // Recursive case: check each child
      foreach (var child in item.Children.OrderBy(c => c.Rank)) {
        if (child.ItemTypeId == Cx.ItemTypeIdTodo && child.StatusTypeId == Cx.StatusTypeIdNotStarted) {
          var result = await GetNextTodoItemRecursive(child.Id, cancellationToken);
          if (result != null) {
            return result;
          }
        }
      }

      // If no suitable child found, return null
      return item;
    } 

  }
}
