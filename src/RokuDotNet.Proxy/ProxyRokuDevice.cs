using System;
using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Client;
using RokuDotNet.Client.Input;
using RokuDotNet.Client.Query;

namespace RokuDotNet.Proxy
{
    public sealed class ProxyRokuDevice : IRokuDevice, IRokuDeviceInput, IRokuDeviceQuery
    {
        private readonly IRokuRpcClient rpc;

        public ProxyRokuDevice(string id, IRokuRpcClient rpc)
        {
            this.rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));

            this.Id = id;
        }

        #region IRokuDevice Members

        public string Id { get; }

        public IRokuDeviceInput Input => this;

        public IRokuDeviceQuery Query => this;

        #endregion

        #region IRokuDeviceInput Members

        Task<GetActiveAppResult> IRokuDeviceQuery.GetActiveAppAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<GetActiveTvChannelResult> IRokuDeviceQuery.GetActiveTvChannelAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<GetAppsResult> IRokuDeviceQuery.GetAppsAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<GetAppsResult>(this.Id, "query/apps", cancellationToken);
        }

        Task<DeviceInfo> IRokuDeviceQuery.GetDeviceInfoAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<GetTvChannelsResult> IRokuDeviceQuery.GetTvChannelsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRokuDeviceQuery Members

        Task IRokuDeviceInput.KeyDownAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRokuDeviceInput.KeyDownAsync(char key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRokuDeviceInput.KeyPressAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRokuDeviceInput.KeyPressAsync(char key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRokuDeviceInput.KeyUpAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRokuDeviceInput.KeyUpAsync(char key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

    public sealed class RokuCommand
    {
        public string Command { get; set; }
    }

    public sealed class RokuCommandResponse
    {
        public string Response { get; set; }
    }
}
