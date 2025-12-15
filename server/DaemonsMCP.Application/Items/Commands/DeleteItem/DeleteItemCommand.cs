using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Enums;

namespace DaemonsMCP.Application.Items.Commands.DeleteItem {
  public class DeleteItemCommand : IRequest<bool> {
    public int ItemId { get; set; }
    public DeleteStrategy Strategy { get; set; } = DeleteStrategy.DeleteCascade;
    public DeleteItemCommand(int itemId, DeleteStrategy strategy = DeleteStrategy.DeleteCascade) {
      ItemId = itemId;
      Strategy = strategy;
    }
  }  

}
