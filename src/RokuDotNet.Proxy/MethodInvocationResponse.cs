using Newtonsoft.Json;

namespace RokuDotNet.Proxy
{
    public sealed class MethodInvocationResponse
    {
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}