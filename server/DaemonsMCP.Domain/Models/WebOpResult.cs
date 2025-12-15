using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public class WebOpResult : McpOpResult {

    public string NextToken { get; set; } = "error no token.";

    public static WebOpResult CreateSuccess(string operation, string message, string nextToken, object? data = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error, operation must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(message)) {
        throw new ArgumentException("Error message must be provided", nameof(message));
      }
      if (string.IsNullOrEmpty(nextToken)) {
        throw new ArgumentException("Error nextToken must be provided", nameof(nextToken));
      }
      return new WebOpResult() { 
        Success = true, 
        Operation = operation, 
        Message = message, 
        NextToken = nextToken,
        Data = data 
      };
    }

    public static WebOpResult CreateFailure(string operation, string nextToken, string errorMessage, Exception? exception = null) {
      if (string.IsNullOrEmpty(operation)) {
        throw new ArgumentException("Error operation must be provided", nameof(operation));
      }
      if (string.IsNullOrEmpty(nextToken)) {
        throw new ArgumentException("Error nextToken must be provided", nameof(nextToken));
      }      
      return new() { Success = false, Operation = operation, NextToken = nextToken, ErrorMessage = errorMessage };
    }

    public override string ToString() {
      return JsonSerializer.Serialize<WebOpResult>(this);
    }


  }
}
