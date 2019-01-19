using System;
using System.Linq.Expressions;
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
        public Task HandleQueryActiveAppAsync()
        {
            return this.HandleQueryMethodAsync(
                "query/active-app",
                d => d.Query.GetActiveAppAsync(It.IsAny<CancellationToken>()),
                new GetActiveAppResult
                {
                    ActiveApp = new RokuApp
                    {
                        Name = "app"
                    }
                });
        }

        [Fact]
        public Task HandleQueryAppsAsync()
        {
            return this.HandleQueryMethodAsync(
                "query/apps",
                d => d.Query.GetAppsAsync(It.IsAny<CancellationToken>()),
                new GetAppsResult
                {
                    Apps = new RokuApp[]
                    {
                        new RokuApp
                        {
                            Name = "app"
                        }
                    }
                });
        }

        [Fact]
        public Task HandleQueryDeviceInfoAsync()
        {
            return this.HandleQueryMethodAsync(
                "query/device-info",
                d => d.Query.GetDeviceInfoAsync(It.IsAny<CancellationToken>()),
                new DeviceInfo
                {
                    IsTv = true
                });
        }

        [Fact]
        public Task HandleQueryTvActiveChannelAsync()
        {
            return this.HandleQueryMethodAsync(
                "query/tv-active-channel",
                d => d.Query.GetActiveTvChannelAsync(It.IsAny<CancellationToken>()),
                new GetActiveTvChannelResult
                {
                    ActiveChannel = new ActiveTvChannel
                    {
                        Name = "PBS"
                    }
                });
        }

        [Fact]
        public Task HandleQueryTvChannelsAsync()
        {
            return this.HandleQueryMethodAsync(
                "query/tv-channels",
                d => d.Query.GetTvChannelsAsync(It.IsAny<CancellationToken>()),
                new GetTvChannelsResult
                {
                    Channels = new TvChannel[]
                    {
                        new TvChannel
                        {
                            Name = "PBS"
                        }
                    }
                });
        }

        private async Task HandleQueryMethodAsync<TResult>(string methodName, Expression<Func<IRokuDevice, Task<TResult>>> setupFunc, TResult expectedResult)
        {
            var expectedDeviceId = "device";

            var device = new Mock<IRokuDevice>(MockBehavior.Strict);

            device
                .Setup(setupFunc)
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
                    MethodName = methodName
                });

            device.Verify(setupFunc, Times.Once);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Payload);
            Assert.True(JToken.DeepEquals(JToken.FromObject(expectedResult), response.Payload));
        }
    }
}