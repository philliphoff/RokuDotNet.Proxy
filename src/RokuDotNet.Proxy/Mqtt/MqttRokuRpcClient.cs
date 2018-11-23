using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RokuDotNet.Proxy.Mqtt
{
    public sealed class MqttRokuRpcClient : IRokuRpcClient
    {
        private readonly string clientId;
        private readonly string password;
        private readonly string userName;

        public MqttRokuRpcClient(string clientId, string userName, string password)
        {
            this.clientId = clientId;
            this.password = password;
            this.userName = userName;
        }

        #region IRokuRpc Members

        public async Task<T> InvokeMethodAsync<TMessagePayload, T>(string deviceId, string methodName, TMessagePayload payload, CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();
            
            using (var client = factory.CreateMqttClient())
            {
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("localhost", 1883)
                    .WithClientId(this.clientId)
                    .WithCredentials(this.userName, this.password)
                    .Build();

                string requestId = Guid.NewGuid().ToString();
                string responseTopic = $"roku/devices/{deviceId}/{methodName}/{requestId}";

                client.Connected += async (s, e) =>
                {
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(responseTopic).Build()).ConfigureAwait(false);
                };           

                var tcs = new TaskCompletionSource<T>();

                client.ApplicationMessageReceived += (s, e) =>
                {
                    string invocationResponseString = e.ApplicationMessage.ConvertPayloadToString();
                    var invocationResponse = JsonConvert.DeserializeObject<MethodInvocationResponse>(invocationResponseString);
                    tcs.TrySetResult(invocationResponse.Payload.ToObject<T>());
                };

                await client.ConnectAsync(options).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                string invocationString = JsonConvert.SerializeObject(
                    new MqttMethodInvocation
                    {
                        DeviceId = deviceId,
                        MethodName = methodName,
                        Payload = JToken.FromObject(payload),
                        ResponseTopic = responseTopic
                    });

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"roku/devices/{deviceId}")
                    .WithPayload(invocationString)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                await client.PublishAsync(message).ConfigureAwait(false);

                using (var cancellationRegistration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}