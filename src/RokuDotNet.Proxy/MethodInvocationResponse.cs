using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RokuDotNet.Proxy
{
    public sealed class MethodInvocationResponse
    {
        [JsonProperty("payload")]
        public JToken Payload { get; set; }
    }
}