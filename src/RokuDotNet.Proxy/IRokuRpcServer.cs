using System;
using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    public interface IRokuRpcServer : IDisposable
    {
        Task StartListeningAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task StopListeningAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}