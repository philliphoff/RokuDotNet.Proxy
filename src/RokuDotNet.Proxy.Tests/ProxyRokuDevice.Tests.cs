using Moq;
using RokuDotNet.Client.Query;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RokuDotNet.Proxy.Tests
{
    public sealed class ProxyRokuDeviceTests
    {
        [Fact]
        public async Task GetActiveAppAsync()
        {
            string deviceId = "deviceId";
            var activeAppResult = new GetActiveAppResult
            {
            };

            var client = new Mock<IRokuRpcClient>(MockBehavior.Strict);

            client.Setup(c => c.InvokeMethodAsync<object, GetActiveAppResult>(deviceId, "query/active-app", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(activeAppResult);

            var device = new ProxyRokuDevice(deviceId, client.Object);

            var result = await device.Query.GetActiveAppAsync();

            Assert.Same(activeAppResult, result);
        }
    }
}