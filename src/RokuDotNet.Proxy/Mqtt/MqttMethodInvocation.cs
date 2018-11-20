using Newtonsoft.Json;

namespace RokuDotNet.Proxy.Mqtt
{
    public sealed class MqttMethodInvocation : MethodInvocation
    {
        [JsonProperty("responseTopic")]
        public string ResponseTopic { get; set; }
    }
}