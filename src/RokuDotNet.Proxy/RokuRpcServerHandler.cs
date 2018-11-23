using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RokuDotNet.Client;

namespace RokuDotNet.Proxy
{
    public sealed class RokuRpcServerHandler : IRokuRpcServerHandler
    {
        private readonly Func<string, Task<IRokuDevice>> deviceMapFunc;

        public RokuRpcServerHandler(Func<string, Task<IRokuDevice>> deviceMapFunc)
        {
            this.deviceMapFunc = deviceMapFunc ?? throw new ArgumentNullException(nameof(deviceMapFunc));
        }

        #region IRokuRpcServerHandler Members

        public Task<MethodInvocationResponse> HandleMethodInvocationAsync(MethodInvocation invocation, CancellationToken cancellationToken)
        {
            switch (invocation.MethodName)
            {
                case "query/apps":

                    return OnQueryApps(invocation, cancellationToken);

                default:

                    break; 
            }

            throw new NotImplementedException();
        }

        #endregion

        private async Task<MethodInvocationResponse> OnQueryApps(MethodInvocation invocation, CancellationToken cancellationToken)
        {
            var device = await this.deviceMapFunc(invocation.DeviceId);

            var result = await device.Query.GetAppsAsync(cancellationToken);

            return new MethodInvocationResponse
            {
                Payload = JToken.FromObject(result)
            };
        }
    }
}