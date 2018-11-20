using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    public interface IRokuRpcClient
    {
        Task<T> InvokeMethodAsync<T>(string deviceId, string methodName, CancellationToken cancellationToken = default(CancellationToken));
    }
}