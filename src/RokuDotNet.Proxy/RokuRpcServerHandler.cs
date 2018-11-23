using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RokuDotNet.Client;

namespace RokuDotNet.Proxy
{
    public sealed class RokuRpcServerHandler : IRokuRpcServerHandler
    {
        private delegate Task<object> HandlerFunc(MethodInvocation invocation, IRokuDevice device, CancellationToken cancellationToken);

        private static readonly IDictionary<string, HandlerFunc> handlers = new Dictionary<string, HandlerFunc>
        {
            { "query/active-app", (invocation, device, cancellationToken) => { return device.Query.GetActiveAppAsync().ToTaskObject(); } },
            { "query/apps", (invocation, device, cancellationToken) => { return device.Query.GetAppsAsync().ToTaskObject(); } }
        };

        private readonly Func<string, Task<IRokuDevice>> deviceMapFunc;

        public RokuRpcServerHandler(Func<string, Task<IRokuDevice>> deviceMapFunc)
        {
            this.deviceMapFunc = deviceMapFunc ?? throw new ArgumentNullException(nameof(deviceMapFunc));
        }

        #region IRokuRpcServerHandler Members

        public async Task<MethodInvocationResponse> HandleMethodInvocationAsync(MethodInvocation invocation, CancellationToken cancellationToken)
        {
            if (handlers.TryGetValue(invocation.MethodName, out HandlerFunc handler))
            {
                var device = await this.deviceMapFunc(invocation.DeviceId).ConfigureAwait(false);

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