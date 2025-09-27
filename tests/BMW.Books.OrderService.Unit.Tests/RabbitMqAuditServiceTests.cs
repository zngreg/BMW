using System.Threading.Tasks;
using BMW.Books.OrderService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class RabbitMqAuditServiceTests
    {
        [Test]
        public async Task SendAuditAsync_LogsMessage()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["RABBITMQ_HOST"]).Returns("localhost");
            configMock.Setup(c => c["RABBITMQ_QUEUE"]).Returns("audit-queue");
            configMock.Setup(c => c["RABBITMQ_USER"]).Returns("guest");
            configMock.Setup(c => c["RABBITMQ_PASS"]).Returns("guest");
            var loggerMock = new Mock<ILogger<RabbitMqAuditService>>();
            var service = new RabbitMqAuditService(configMock.Object, loggerMock.Object);
            // We can't easily test RabbitMQ connection in unit test, so just check logging
            await service.SendAuditAsync("test message");
            loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("test message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}
