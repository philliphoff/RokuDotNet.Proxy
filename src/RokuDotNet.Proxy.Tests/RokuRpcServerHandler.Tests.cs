using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RokuDotNet.Client;
using RokuDotNet.Client.Query;
using Xunit;

namespace RokuDotNet.Proxy.Tests
{
    public sealed class RokuRpcServerHandlerTests
    {
        [Fact]
        public async Task HandleQueryActiveAppAsync()
        {
            var expectedDeviceId = "device";
            var expectedResult = new GetActiveAppResult
            {
                ActiveApp = new RokuApp
                {
                    Name = "app"
                }
            };

            var device = new Mock<IRokuDevice>(MockBehavior.Strict);

            device
                .Setup(d => d.Query.GetActiveAppAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            Func<string, CancellationToken, Task<IRokuDevice>> deviceMapFunc =
                (string deviceId, CancellationToken cancellationToken) =>
                {
                    Assert.Equal(expectedDeviceId, deviceId);

                    return Task.FromResult(device.Object);
                };

            var handler = new RokuRpcServerHandler(deviceMapFunc);

            var response = await handler.HandleMethodInvocationAsync(
                new MethodInvocation
                {
                    DeviceId = expectedDeviceId,
                    MethodName = "query/active-app"
                });

            device.Verify(d => d.Query.GetActiveAppAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Payload);
            Assert.True(JToken.DeepEquals(JToken.FromObject(expectedResult), response.Payload));
        }

        [Fact]
        public async Task HandleQueryAppsAsync()
        {
            var expectedDeviceId = "device";
            var expectedResult = new GetAppsResult
            {
                Apps = new RokuApp[]
                {
                    new RokuApp
                    {
                        Name = "app"
                    }
                }
            };

            var device = new Mock<IRokuDevice>(MockBehavior.Strict);

            device
                .Setup(d => d.Query.GetAppsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            Func<string, CancellationToken, Task<IRokuDevice>> deviceMapFunc =
                (string deviceId, CancellationToken cancellationToken) =>
                {
                    Assert.Equal(expectedDeviceId, deviceId);

                    return Task.FromResult(device.Object);
                };

            var handler = new RokuRpcServerHandler(deviceMapFunc);

            var response = await handler.HandleMethodInvocationAsync(
                new MethodInvocation
                {
                    DeviceId = expectedDeviceId,
                    MethodName = "query/apps"
                });

            device.Verify(d => d.Query.GetAppsAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Payload);
            Assert.True(JToken.DeepEquals(JToken.FromObject(expectedResult), response.Payload));
        }
    }
}