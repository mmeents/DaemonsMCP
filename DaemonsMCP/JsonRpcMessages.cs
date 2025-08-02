using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP
{
    // JSON-RPC request structure
    public class JsonRpcRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
        [JsonPropertyName("method")]
        public string? Method { get; set; }
        [JsonPropertyName("params")]
        public JsonElement? Params { get; set; }
        [JsonPropertyName("id")]
        public JsonElement? Id { get; set; }
    }

    // JSON-RPC response structure
    public class JsonRpcInitResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
        [JsonPropertyName("result")]
        public object? Result { get; set; }        

        [JsonPropertyName("id")]
        public JsonElement? Id { get; set; }
    }

    // JSON-RPC response structure
    public class JsonRpcResponse : JsonRpcInitResponse
    {        
        [JsonPropertyName("error")]
        public object? Error { get; set; }     
    }

    

}
