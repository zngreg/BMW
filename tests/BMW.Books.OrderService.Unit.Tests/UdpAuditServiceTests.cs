using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BMW.Books.OrderService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class UdpAuditServiceTests
    {
        [Test]
        public async Task SendAuditAsync_SendsUdpPacket()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_HOST"]).Returns("127.0.0.1");
            configMock.Setup(c => c["AUDIT_PORT"]).Returns("5140");
            var service = new UdpAuditService(configMock.Object);

            using var udpListener = new UdpClient(5140);
            var receiveTask = udpListener.ReceiveAsync();
            await service.SendAuditAsync("test message");
            var result = await receiveTask;
            var msg = Encoding.UTF8.GetString(result.Buffer);
            Assert.That(msg, Does.Contain("test message"));
        }
    }
}
