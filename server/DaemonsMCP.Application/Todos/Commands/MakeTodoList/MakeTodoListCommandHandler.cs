using MediatR;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.Todos.Commands.MakeTodoList {

  public class MakeTodoListCommand : IRequest<ItemDto?> {    
    public string TodoTitle { get; set; } = String.Empty;
    public string[] TodoItems { get; set; } = Array.Empty<string>();
    public int? ParentItemId { get; set; } = null;
    public MakeTodoListCommand() {}
    public MakeTodoListCommand(string todoTitle, string[] todoItems, int? itemId = null) {
      TodoTitle = todoTitle;
      TodoItems = todoItems;
      ParentItemId = itemId;
    }
  }

  // MakeTodoList command handler creates a todo list item with sub-items if they do not already exist.
  // it only creates the name and skips on matching existing items. so names need to be unique within the todo tree.
  // they can be added to any arbitrary depth with the names only in the Todo tree.
  public class MakeTodoListCommandHandler(
    IItemRepository itemRepository  
  ) : IRequestHandler<MakeTodoListCommand, ItemDto?> {
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<ItemDto?> Handle(MakeTodoListCommand request, CancellationToken cancellationToken) {
      
      // note filters by Type todo and status not done in the repo.
      var todoItem = await _itemRepository.GetByTodoNameAsync(request.TodoTitle, cancellationToken);
      if (todoItem is null) {
        var parentItem = null as Item;
        var parentItemId = request.ParentItemId ?? Cx.TodoRootItemId;
        parentItem = await _itemRepository.GetByIdWithChildrenAsync(parentItemId, 1, cancellationToken);

        todoItem = new Item( request.TodoTitle, string.Empty,
          Cx.ItemTypeIdTodo, 
          Cx.StatusTypeIdNotStarted, 
          parentItem?.Children.Count()+1 ?? 1,
          parentItem?.Id ?? Cx.TodoRootItemId, 
          null, null);
        await _itemRepository.AddAsync(todoItem, cancellationToken);
        await _itemRepository.SaveChangesAsync(cancellationToken);
      } 

      var subItemAdded = false;
      int rankRange = 1;
      foreach (var itemName in request.TodoItems) {
        var existingSubItem = todoItem
          .Children.FirstOrDefault(c => c.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (existingSubItem is null) {
          var todoSubItem = new Item(itemName, string.Empty,
            Cx.ItemTypeIdTodo, Cx.StatusTypeIdNotStarted, 
            rankRange, 
            todoItem.Id, 
            null, null);
          await _itemRepository.AddAsync(todoSubItem, cancellationToken);
          subItemAdded = true;
        }
        rankRange++;
      }
      if (subItemAdded) {
        await _itemRepository.SaveChangesAsync(cancellationToken);
      }

      var returnTodo = await _itemRepository.GetByIdWithChildrenAsync(todoItem.Id, 1, cancellationToken);

      return returnTodo?.ToDto();
    }
  }
}
