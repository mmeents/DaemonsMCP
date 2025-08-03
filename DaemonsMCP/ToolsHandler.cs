using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP
{
    public static class Tx {
        public const string ListMethods = "tools/list";
        public const string CallMethod = "tools/call";
        public const string Initialize = "initialize";
        public const string InitializedNotification = "notifications/initialized"; 
        public const string ListResources = "resources/list";
        public const string PromptsList = "prompts/list";
    }

}
