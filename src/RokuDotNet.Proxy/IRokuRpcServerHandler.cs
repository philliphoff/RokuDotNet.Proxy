using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    public interface IRokuRpcServerHandler
    {
        Task<MethodInvocationResponse> HandleMethodInvocationAsync(MethodInvocation invocation, CancellationToken cancellationToken = default(CancellationToken));
    }
}