using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    public interface IRokuRpcClient
    {
        Task<TMethodResponsePayload> InvokeMethodAsync<TMethodPayload, TMethodResponsePayload>(string deviceId, string methodName, TMethodPayload payload, CancellationToken cancellationToken = default(CancellationToken));
    }
}