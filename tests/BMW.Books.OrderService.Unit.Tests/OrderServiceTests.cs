using System;
using System.Net.Http;
using System.Threading.Tasks;
using BMW.Books.OrderService.Models;
using BMW.Books.OrderService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IConfiguration> _configMock;
        private Mock<IAuditService> _auditServiceMock;
        private Mock<IAuditServiceFactory> _auditFactoryMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private IOrderService _service;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _auditFactoryMock = new Mock<IAuditServiceFactory>();
            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(_auditServiceMock.Object);
            _configMock.Setup(c => c["BOOKS_BASE_URL"]).Returns("http://fake-books");
            _service = new Services.OrderService(_httpClientFactoryMock.Object, _configMock.Object, _auditFactoryMock.Object, _serviceProviderMock.Object);
        }

        [Test]
        public void GetOrderById_ReturnsNullIfNotFound()
        {
            var result = _service.GetOrderById("notfound");
            Assert.That(result, Is.Null);
        }

        // [Test]
        // public async Task CreateOrderAsync_ReturnsNullIfBookNotFound()
        // {
        //     _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
        //     _auditServiceMock = new Mock<IAuditService>();
        //     _service = new Services.OrderService(_httpClientFactoryMock.Object, _configMock.Object, _auditFactoryMock.Object, _serviceProviderMock.Object);

        //     var req = new OrderRequest("b1", 1);
        //     var clientMock = new Mock<HttpMessageHandler>();
        //     var httpClient = new HttpClient(clientMock.Object);
        //     _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        //     var result = await _service.CreateOrderAsync(req);
        //     Assert.That(result, Is.Null);
        // }
    }
}
