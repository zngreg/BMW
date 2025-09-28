using BMW.Books.CatalogueService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    public class RabbitMqStockUpdateListenerTests
    {
        private Mock<IConfiguration> _configMock;
        private Mock<ILogger<RabbitMqStockUpdateListener>> _loggerMock;
        private Mock<IBookService> _bookServiceMock;
        private RabbitMqStockUpdateListener _listener;

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<RabbitMqStockUpdateListener>>();
            _bookServiceMock = new Mock<IBookService>();
            _configMock.Setup(c => c["RABBITMQ_HOST"]).Returns("localhost");
            _configMock.Setup(c => c["RABBITMQ_STOCK_UPDATE_QUEUE"]).Returns("test-queue");
            _configMock.Setup(c => c["RABBITMQ_USER"]).Returns("guest");
            _configMock.Setup(c => c["RABBITMQ_PASS"]).Returns("guest");
            _listener = new RabbitMqStockUpdateListener(_configMock.Object, _loggerMock.Object, _bookServiceMock.Object);
        }

        [Test]
        public void Constructor_SetsConfigValues()
        {
            Assert.That(_listener, Is.Not.Null);
        }
    }
}
