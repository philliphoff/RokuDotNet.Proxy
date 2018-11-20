using System.Threading;
using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    public sealed class RokuRpcServerHandler : IRokuRpcServerHandler
    {
        #region IRokuRpcServerHandler Members

        public Task<MethodInvocationResponse> HandleMethodInvocationAsync(MethodInvocation invocation, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}