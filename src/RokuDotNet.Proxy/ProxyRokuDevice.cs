using System;
using System.Linq;
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

        #region IRokuDeviceQuery Members

        Task<GetActiveAppResult> IRokuDeviceQuery.GetActiveAppAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<object, GetActiveAppResult>(this.Id, "query/active-app", new Object(), cancellationToken);
        }

        Task<GetActiveTvChannelResult> IRokuDeviceQuery.GetActiveTvChannelAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<object, GetActiveTvChannelResult>(this.Id, "query/tv-active-channel", new Object(), cancellationToken);
        }

        Task<GetAppsResult> IRokuDeviceQuery.GetAppsAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<object, GetAppsResult>(this.Id, "query/apps", new Object(), cancellationToken);
        }

        Task<DeviceInfo> IRokuDeviceQuery.GetDeviceInfoAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<object, DeviceInfo>(this.Id, "query/device-info", new Object(), cancellationToken);
        }

        Task<GetTvChannelsResult> IRokuDeviceQuery.GetTvChannelsAsync(CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<object, GetTvChannelsResult>(this.Id, "query/tv-channels", new Object(), cancellationToken);
        }

        #endregion

        #region IRokuDeviceInput Members

        Task IRokuDeviceInput.KeyDownAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keydown/key", key.ToString(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyDownAsync(char key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keydown/key", key.ToString(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyPressAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keypress/key", key.ToString(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyPressAsync(SpecialKeys[] keys, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string[], object>(this.Id, "keypress/keys/special", keys.Select(key => key.ToString()).ToArray(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyPressAsync(char key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keypress/key", key.ToString(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyPressAsync(char[] keys, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string[], object>(this.Id, "keypress/keys/literal", keys.Select(key => key.ToString()).ToArray(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyUpAsync(SpecialKeys key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keyup/key", key.ToString(), cancellationToken);
        }

        Task IRokuDeviceInput.KeyUpAsync(char key, CancellationToken cancellationToken)
        {
            return this.rpc.InvokeMethodAsync<string, object>(this.Id, "keyup/key", key.ToString(), cancellationToken);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
