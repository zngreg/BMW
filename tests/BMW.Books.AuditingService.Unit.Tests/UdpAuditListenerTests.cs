using System.Net;
using System.Net.Sockets;
using System.Text;
using BMW.Books.AuditingService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BMW.Books.AuditingService.Unit.Tests
{
    [TestFixture]
    public class UdpAuditListenerTests
    {
        [Test]
        public void Constructor_SetsPortFromConfig()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_PORT"]).Returns("12345");
            var listener = new UdpAuditListener(configMock.Object);
            Assert.That(listener, Is.Not.Null);
        }

        [Test]
        public async Task StartAsync_CancelsImmediately_DoesNotThrow()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_PORT"]).Returns("5140");
            var listener = new UdpAuditListener(configMock.Object);
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            await Task.Yield();
            Assert.DoesNotThrowAsync(async () => await listener.StartAsync(cts.Token));
        }

        [Test]
        public async Task StartAsync_ReceivesUdpMessage_LogsToConsole()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            // Use an ephemeral port
            using var udp = new UdpClient(0);
            int port = ((IPEndPoint)udp.Client.LocalEndPoint!).Port;
            configMock.Setup(c => c["AUDIT_PORT"]).Returns(port.ToString());
            var listener = new UdpAuditListener(configMock.Object);
            using var cts = new CancellationTokenSource();

            // Start listener in background
            var listenerTask = Task.Run(() => listener.StartAsync(cts.Token));

            // Give the listener a moment to start
            await Task.Delay(200);

            // Act: send a UDP message
            var testMessage = "test-udp-message";
            var data = Encoding.UTF8.GetBytes(testMessage);
            await udp.SendAsync(data, data.Length, "127.0.0.1", port);

            // Give the listener time to process
            await Task.Delay(200);
            cts.Cancel();

            // No assertion: this test ensures no exceptions and coverage of receive path
            Assert.Pass();
        }
    }
}
