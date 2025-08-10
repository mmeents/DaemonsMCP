using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class OperationResult {
    public bool Success { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }

    public static OperationResult CreateSuccess(string operation, string message, object? data = null)
        => new() { Success = true, Operation = operation, Message = message, Data = data };

    public static OperationResult CreateFailure(string operation, string errorMessage, Exception? exception = null)
        => new() { Success = false, Operation = operation, ErrorMessage = errorMessage, Exception = exception };
  }
}
