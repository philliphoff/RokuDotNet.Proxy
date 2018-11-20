using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace RokuDotNet.Proxy.Mqtt
{
    public sealed class MqttRokuRpcServer : IRokuRpcServer
    {
        private readonly string deviceId;
        private readonly IRokuRpcServerHandler handler;

        private readonly SemaphoreSlim listenLock = new SemaphoreSlim(1, 1);

        private IMqttClient mqttClient;
        
        public MqttRokuRpcServer(string deviceId, IRokuRpcServerHandler handler)
        {
            this.deviceId = deviceId;
            this.handler = handler;
        }

        #region IRokuRpcServer Members

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            if (this.mqttClient == null)
            {
                await this.listenLock.WaitAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    if (this.mqttClient == null)
                    {
                        var factory = new MqttFactory();

                        var client = factory.CreateMqttClient();

                        var options = new MqttClientOptionsBuilder()
                            .WithTcpServer("localhost", 1883)
                            .Build();

                        var tcs = new TaskCompletionSource<bool>();

                        client.Connected += async (s, e) =>
                        {
                            client.ApplicationMessageReceived += this.OnApplicationMessageReceived;

                            await client.SubscribeAsync(new TopicFilterBuilder().WithTopic($"roku/devices/{this.deviceId}").Build()).ConfigureAwait(false);
                        
                            tcs.TrySetResult(true);
                        };

                        await client.ConnectAsync(options).ConfigureAwait(false);

                        await tcs.Task.ConfigureAwait(false);
                    }
                }
                finally
                {
                    this.listenLock.Release();
                }
            }
        }

        public async Task StopListeningAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.mqttClient != null)
            {
                await this.listenLock.WaitAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    if (this.mqttClient != null)
                    {
                        await this.mqttClient.DisconnectAsync().ConfigureAwait(false);

                        this.mqttClient.Dispose();

                        this.mqttClient = null;
                    }
                }
                finally
                {
                    this.listenLock.Release();
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.mqttClient?.Dispose();
        }

        #endregion

        private async void OnApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string invocationString = e.ApplicationMessage.ConvertPayloadToString();
            var invocation = JsonConvert.DeserializeObject<MqttMethodInvocation>(invocationString);

            using (var tokenSource = new CancellationTokenSource())
            {
                var invocationResponse = await this.handler.HandleMethodInvocationAsync(invocation, tokenSource.Token).ConfigureAwait(false);
                var invocationResponseString = JsonConvert.SerializeObject(invocationResponse);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(invocation.ResponseTopic)
                    .WithPayload(invocationResponseString)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                await this.mqttClient.PublishAsync(message).ConfigureAwait(false);
            }
        }
    }
}