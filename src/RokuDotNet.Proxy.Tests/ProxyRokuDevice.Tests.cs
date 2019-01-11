using Moq;
using RokuDotNet.Client;
using RokuDotNet.Client.Input;
using RokuDotNet.Client.Query;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RokuDotNet.Proxy.Tests
{
    public sealed class ProxyRokuDeviceTests
    {
        [Fact]
        public Task QueryGetActiveAppAsync()
        {
            return TestQueryMethodAsync(device => device.Query.GetActiveAppAsync(), "query/active-app", new GetActiveAppResult());
        }

        [Fact]
        public Task QueryGetActiveTvChannelAsync()
        {
            return TestQueryMethodAsync(device => device.Query.GetActiveTvChannelAsync(), "query/tv-active-channel", new GetActiveTvChannelResult());
        }

        [Fact]
        public Task QueryGetAppsAsync()
        {
            return TestQueryMethodAsync(device => device.Query.GetAppsAsync(), "query/apps", new GetAppsResult());
        }

        [Fact]
        public Task QueryGetDeviceInfoAsync()
        {
            return TestQueryMethodAsync(device => device.Query.GetDeviceInfoAsync(), "query/device-info", new DeviceInfo());
        }

        [Fact]
        public Task QueryGetTvChannelsAsync()
        {
            return TestQueryMethodAsync(device => device.Query.GetTvChannelsAsync(), "query/tv-channels", new GetTvChannelsResult());
        }

        [Fact]
        public Task InputKeyDownSpecialKeysAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyDownAsync(SpecialKeys.VolumeUp), "keydown/key", "VolumeUp");
        }

        [Fact]
        public Task InputKeyDownCharAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyDownAsync('z'), "keydown/key", "z");
        }

        [Fact]
        public Task InputKeyPressSpecialKeysAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyPressAsync(SpecialKeys.VolumeUp), "keypress/key", "VolumeUp");
        }

        [Fact]
        public Task InputKeyPressSpecialKeysArrayAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyPressAsync(new[] { SpecialKeys.VolumeUp, SpecialKeys.VolumeDown }), "keypress/keys/special", new[] { "VolumeUp", "VolumeDown" });
        }

        [Fact]
        public Task InputKeyPressCharAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyPressAsync('z'), "keypress/key", "z");
        }

        [Fact]
        public Task InputKeyPressCharArrayAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyPressAsync(new[] { 'z', 'a' }), "keypress/keys/literal", new[] { "z", "a" });
        }

        [Fact]
        public Task InputKeyUpSpecialKeysAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyUpAsync(SpecialKeys.VolumeUp), "keyup/key", "VolumeUp");
        }

        [Fact]
        public Task InputKeyUpCharAsync()
        {
            return TestInputMethodAsync(device => device.Input.KeyUpAsync('z'), "keyup/key", "z");
        }

        private async Task TestInputMethodAsync<TRequest>(Func<IRokuDevice, Task> methodFunc, string expectedMethodName, TRequest expectedRequest)
        {
            string deviceId = "deviceId";

            var client = new Mock<IRokuRpcClient>(MockBehavior.Strict);

            client.Setup(c => c.InvokeMethodAsync<TRequest, object>(deviceId, expectedMethodName, expectedRequest, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Task.FromResult(new object()));

            var device = new ProxyRokuDevice(deviceId, client.Object);

            await methodFunc(device);
        }

        private async Task TestQueryMethodAsync<TResult>(Func<IRokuDevice, Task<TResult>> methodFunc, string expectedMethodName, TResult expectedResult)
        {
            string deviceId = "deviceId";

            var client = new Mock<IRokuRpcClient>(MockBehavior.Strict);

            client.Setup(c => c.InvokeMethodAsync<object, TResult>(deviceId, expectedMethodName, It.IsAny<object>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(expectedResult);

            var device = new ProxyRokuDevice(deviceId, client.Object);

            var actualResult = await methodFunc(device);

            Assert.Same(expectedResult, actualResult);
        }
    }
}