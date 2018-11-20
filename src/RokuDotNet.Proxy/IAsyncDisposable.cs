using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    internal interface IAsyncDisposable
    {
        Task DisposeAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}