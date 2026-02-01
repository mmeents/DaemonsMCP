using MediatR;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;


namespace DaemonsMCP.Application.Todos.Commands.MarkTodo {
    public class MarkTodoCancelCommand : IRequest<ItemDto?> {
        public int ItemId { get; }
        
        public MarkTodoCancelCommand(int itemId) {
        ItemId = itemId;
        }
  }

  public class MarkTodoCancelCommandHandler : IRequestHandler<MarkTodoCancelCommand, ItemDto?> {
    private readonly IItemRepository _itemRepository;
    public MarkTodoCancelCommandHandler(IItemRepository itemRepository) {
      _itemRepository = itemRepository;
    }
    public async Task<ItemDto?> Handle(MarkTodoCancelCommand request, CancellationToken cancellationToken) {
      var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
      if (item == null) {
        return null;
      }
      item.UpdateStatus(Cx.StatusTypeIdCancelled);
      await _itemRepository.UpdateAsync(item, cancellationToken);
      await _itemRepository.SaveChangesAsync(cancellationToken);
      return item.ToDto();
    }
  }
}
