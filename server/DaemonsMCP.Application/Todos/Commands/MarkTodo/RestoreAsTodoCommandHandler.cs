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
    public class RestoreAsTodoCommand : IRequest<ItemDto?> {
        public int ItemId { get; }
    
        public RestoreAsTodoCommand(int itemId) {
        ItemId = itemId;
        }
  }

  public class RestoreAsTodoCommandHandler : IRequestHandler<RestoreAsTodoCommand, ItemDto?> {
    private readonly IItemRepository _itemRepository;

    public RestoreAsTodoCommandHandler(IItemRepository itemRepository) {
      _itemRepository = itemRepository;
    }

    public async Task<ItemDto?> Handle(RestoreAsTodoCommand request, CancellationToken cancellationToken) {
      var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
      if (item == null) {
        return null;
      }

      item.UpdateStatus(Cx.StatusTypeIdNotStarted);
      await _itemRepository.UpdateAsync(item, cancellationToken);
      await _itemRepository.SaveChangesAsync(cancellationToken);

      return item.ToDto();
    }
  }
}
