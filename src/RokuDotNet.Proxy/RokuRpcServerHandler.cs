using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RokuDotNet.Client;
using RokuDotNet.Client.Input;

namespace RokuDotNet.Proxy
{
    public sealed class RokuRpcServerHandler : IRokuRpcServerHandler
    {
        private delegate Task<object> HandlerFunc(MethodInvocation invocation, IRokuDevice device, CancellationToken cancellationToken);

        private static readonly IDictionary<string, HandlerFunc> handlers = new Dictionary<string, HandlerFunc>
        {
            { "query/active-app",           (invocation, device, cancellationToken) => device.Query.GetActiveAppAsync(cancellationToken).ToTaskObject() },
            { "query/apps",                 (invocation, device, cancellationToken) => device.Query.GetAppsAsync(cancellationToken).ToTaskObject() },
            { "query/device-info",          (invocation, device, cancellationToken) => device.Query.GetDeviceInfoAsync(cancellationToken).ToTaskObject() },
            { "query/tv-active-channel",    (invocation, device, cancellationToken) => device.Query.GetActiveTvChannelAsync(cancellationToken).ToTaskObject() },
            { "query/tv-channels",          (invocation, device, cancellationToken) => device.Query.GetTvChannelsAsync(cancellationToken).ToTaskObject() },

            { "keydown/key",    OnKeyMethod(device => device.Input.KeyDownAsync, device => device.Input.KeyDownAsync) },
            { "keypress/key",   OnKeyMethod(device => device.Input.KeyPressAsync, device => device.Input.KeyPressAsync) },
            { "keyup/key",      OnKeyMethod(device => device.Input.KeyUpAsync, device => device.Input.KeyUpAsync) },

            { "keypress/keys/literal", OnLiteralKeysMethod },
            { "keypress/keys/special", OnSpecialKeysMethod }
        };

        private static Task<object> OnLiteralKeysMethod(MethodInvocation invocation, IRokuDevice device, CancellationToken cancellationToken)
        {
            var keys = invocation.Payload.Values<string>().Select(keyString => keyString[0]).ToArray();

            return device.Input.KeyPressAsync(keys, cancellationToken).ToTaskObject();
        }

        private static Task<object> OnSpecialKeysMethod(MethodInvocation invocation, IRokuDevice device, CancellationToken cancellationToken)
        {
            var keys = invocation.Payload.Values<string>().Select(keyString => (SpecialKeys)Enum.Parse(typeof(SpecialKeys), keyString)).ToArray();

            return device.Input.KeyPressAsync(keys, cancellationToken).ToTaskObject();
        }

        private static HandlerFunc OnKeyMethod(Func<IRokuDevice, Func<SpecialKeys, CancellationToken, Task>> specialKeyFunc, Func<IRokuDevice, Func<char, CancellationToken, Task>> charFunc)
        {
            return (invocation, device, cancellationToken) =>
            {
                string keyString = (string)invocation.Payload;
        
                if (Enum.TryParse<SpecialKeys>(keyString, out SpecialKeys key))
                {
                    return specialKeyFunc(device).Invoke(key, cancellationToken).ToTaskObject();
                }
                else
                {
                    return charFunc(device).Invoke(keyString[0], cancellationToken).ToTaskObject();
                }
            };
        }

        private readonly Func<string, CancellationToken, Task<IRokuDevice>> deviceMapFunc;

        public RokuRpcServerHandler(Func<string, CancellationToken, Task<IRokuDevice>> deviceMapFunc)
        {
            this.deviceMapFunc = deviceMapFunc ?? throw new ArgumentNullException(nameof(deviceMapFunc));
        }

        #region IRokuRpcServerHandler Members

        public async Task<MethodInvocationResponse> HandleMethodInvocationAsync(MethodInvocation invocation, CancellationToken cancellationToken)
        {
            if (handlers.TryGetValue(invocation.MethodName, out HandlerFunc handler))
            {
                var device = await this.deviceMapFunc(invocation.DeviceId, cancellationToken).ConfigureAwait(false);

                var result = await handler(invocation, device, cancellationToken).ConfigureAwait(false);

                return new MethodInvocationResponse
                {
                    Payload = JToken.FromObject(result)
                };
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}