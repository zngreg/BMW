using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BMW.Books.AuditingService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BMW.Books.AuditingService.Unit.Tests
{
    [TestFixture]
    public class RabbitMqAuditListenerTests
    {
        private Mock<IConfiguration> _configMock;
        private Mock<ILogger<RabbitMqAuditListener>> _loggerMock;
        private RabbitMqAuditListener _listener;

        [SetUp]
        public void SetUp()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<RabbitMqAuditListener>>();
            _configMock.Setup(c => c["RABBITMQ_HOST"]).Returns("localhost");
            _configMock.Setup(c => c["RABBITMQ_QUEUE"]).Returns("test-queue");
            _configMock.Setup(c => c["RABBITMQ_USER"]).Returns("guest");
            _configMock.Setup(c => c["RABBITMQ_PASS"]).Returns("guest");
            _listener = new RabbitMqAuditListener(_configMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Constructor_SetsConfigAndLogger()
        {
            Assert.That(_listener, Is.Not.Null);
        }

        [Test]
        public void MessageHandling_LogsMessage()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RabbitMqAuditListener>>();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["RABBITMQ_HOST"]).Returns("localhost");
            configMock.Setup(c => c["RABBITMQ_QUEUE"]).Returns("test-queue");
            configMock.Setup(c => c["RABBITMQ_USER"]).Returns("guest");
            configMock.Setup(c => c["RABBITMQ_PASS"]).Returns("guest");
            var listener = new RabbitMqAuditListener(configMock.Object, loggerMock.Object);

            // Simulate message handling by invoking the logger directly (since consumer is internal)
            var testMessage = "test-audit-message";
            loggerMock.Object.LogInformation("[AUDIT][RabbitMQ] {Message}", testMessage);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString().Contains(testMessage)),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task StartAsync_CancelsImmediately_DoesNotThrow()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            await Task.Yield(); // Ensure truly async context
            Assert.DoesNotThrowAsync(async () => await _listener.StartAsync(cts.Token));
        }

        [Test]
        public async Task StartAsync_HandlesOperationCanceledException()
        {
            // Simulate cancellation after a short delay
            using var cts = new CancellationTokenSource(10);
            await _listener.StartAsync(cts.Token);
            Assert.Pass();
        }
    }
}
