using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class OperationResult {

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("exception")]
    public Exception? Exception { get; set; }

    public static OperationResult CreateSuccess(string operation, string message, object? data = null) { 
      if (string.IsNullOrEmpty(operation)) { 
        throw new ArgumentException("Operation name must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(message)) {
        throw new ArgumentException("Operation name must be provided", nameof(message));
      }
      return new() { Success = true, Operation = operation, Message = message, Data = data };
    }

    public static OperationResult CreateFailure(string operation, string errorMessage, Exception? exception = null) { 
      if (string.IsNullOrEmpty(operation)) { 
          throw new ArgumentException("Operation name must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(errorMessage)) {
        throw new ArgumentException("Operation name must be provided", nameof(errorMessage));
      }
      return new() { Success = false, Operation = operation, ErrorMessage = errorMessage, Exception = exception };
    }

    public override string ToString() {
      return JsonSerializer.Serialize<OperationResult>(this);
    }
  }
}
