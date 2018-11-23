using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RokuDotNet.Proxy
{
    public class MethodInvocation
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("payload")]
        public JToken Payload { get; set; }
    }
}