using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.Todos.Commands.MarkTodo {

  public class MarkTodoDoneCommand : IRequest<ItemDto?> {
    public int ItemId { get; }
    
    public MarkTodoDoneCommand(int itemId) {
    ItemId = itemId;
    }
  }
  internal class MarkTodoDoneCommandHandler : IRequestHandler<MarkTodoDoneCommand, ItemDto?> {
    private readonly IItemRepository _itemRepository;

    public MarkTodoDoneCommandHandler(IItemRepository itemRepository) {
      _itemRepository = itemRepository;
    }

    public async Task<ItemDto?> Handle(MarkTodoDoneCommand request, CancellationToken cancellationToken) {

      var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
      if (item == null) {
        return null;
      }
      item.MarkCompleted();
      item.UpdateStatus(Cx.StatusTypeIdComplete);
      await _itemRepository.UpdateAsync(item, cancellationToken);
      await _itemRepository.SaveChangesAsync(cancellationToken);

      return await _itemRepository.MapToDto(item, 2, cancellationToken);
    }
  }
}
