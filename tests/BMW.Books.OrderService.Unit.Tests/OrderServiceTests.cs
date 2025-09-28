using System.Runtime.InteropServices.Marshalling;
using BMW.Books.OrderService.Models;
using BMW.Books.OrderService.Repositories;
using BMW.Books.OrderService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IConfiguration> _configMock;
        private Mock<IAuditService> _auditServiceMock;
        private Mock<IAuditServiceFactory> _auditFactoryMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IStockUpdateService> _stockUpdateService;
        private Mock<Clients.IBookCatalogClient> _bookCatalogClient;
        private Mock<IOrderRepository> _mockOrderRepository;

        private IOrderService _service;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _configMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _auditFactoryMock = new Mock<IAuditServiceFactory>();
            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(_auditServiceMock.Object);
            _configMock.Setup(c => c["BOOKS_BASE_URL"]).Returns("http://fake-books");
            _stockUpdateService = new Mock<IStockUpdateService>();
            _bookCatalogClient = new Mock<Clients.IBookCatalogClient>();
            _mockOrderRepository = new Mock<IOrderRepository>();

            _service = new Services.OrderService(
                _auditFactoryMock.Object,
                _serviceProviderMock.Object,
                _stockUpdateService.Object,
                _bookCatalogClient.Object,
                _mockOrderRepository.Object
            );
        }

        [Test]
        public async Task CreateOrder_WithValidData_ShouldSucceed()
        {
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                Books = new List<OrderBook>
                    {
                        new OrderBook { BookId = "1234567890", Quantity = 2, UnitPrice = 15.99m }
                    }
            };

            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(_auditServiceMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IStockUpdateService))).Returns(_auditServiceMock.Object);

            _bookCatalogClient.Setup(c => c.GetBookByIdAsync("1234567890")).ReturnsAsync(new ResponseModel<Book?>
            {
                IsSuccess = true,
                Data = new Book("1234567890", "Test Book", "Test Author", 15.99m, 5)
            });

            _serviceProviderMock.Setup(x => x.GetService(typeof(Clients.IBookCatalogClient))).Returns(_bookCatalogClient.Object);

            _mockOrderRepository.Setup(x => x.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync(order);

            _service = new Services.OrderService(
                _auditFactoryMock.Object,
                _serviceProviderMock.Object,
                _stockUpdateService.Object,
                _bookCatalogClient.Object,
                _mockOrderRepository.Object
            );

            var result = await _service.CreateOrderAsync(new OrderRequest
            {
                Books = new List<RequestItem>
                    {
                        new RequestItem { BookId = "1234567890", Quantity = 2 }
                    }
            });

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Books.Count(), Is.EqualTo(1));
            Assert.That(result.Data.Books.First().BookId, Is.EqualTo("1234567890"));
            Assert.That(result.Data.Books.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public void CreateOrder_With_No_Books_Should_Fail()
        {
            var result = _service.CreateOrderAsync(new OrderRequest { Books = new List<RequestItem>() }).Result;
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reason, Is.EqualTo("At least one book is required"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public void CreateOrder_With_0_Books_Quantity_Should_Fail()
        {
            var result = _service.CreateOrderAsync(new OrderRequest { Books = new List<RequestItem> { new RequestItem { BookId = "1234567890", Quantity = 0 } } }).Result;
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reason, Is.EqualTo("Quantity must be greater than zero"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task CreateOrder_With_No_Valid_Books_Should_FailAsync()
        {
            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(_auditServiceMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IStockUpdateService))).Returns(_auditServiceMock.Object);

            _serviceProviderMock.Setup(x => x.GetService(typeof(Clients.IBookCatalogClient))).Returns(_bookCatalogClient.Object);

            _service = new Services.OrderService(
                _auditFactoryMock.Object,
                _serviceProviderMock.Object,
                _stockUpdateService.Object,
                _bookCatalogClient.Object,
                _mockOrderRepository.Object
            );

            var result = await _service.CreateOrderAsync(new OrderRequest
            {
                Books = new List<RequestItem>
                    {
                        new RequestItem { BookId = "1234567890", Quantity = 2 }
                    }
            });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reason, Is.EqualTo("Book with ID 1234567890 not found"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task CreateOrder_With_Invalid_Book_Quantity_More_Than_Stock_Should_FailAsync()
        {
            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(_auditServiceMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IStockUpdateService))).Returns(_auditServiceMock.Object);

            _bookCatalogClient.Setup(c => c.GetBookByIdAsync("1234567890")).ReturnsAsync(new ResponseModel<Book?>
            {
                IsSuccess = true,
                Data = new Book("1234567890", "Test Book", "Test Author", 15.99m, 5)
            });

            _serviceProviderMock.Setup(x => x.GetService(typeof(Clients.IBookCatalogClient))).Returns(_bookCatalogClient.Object);

            _service = new Services.OrderService(
                _auditFactoryMock.Object,
                _serviceProviderMock.Object,
                _stockUpdateService.Object,
                _bookCatalogClient.Object,
                _mockOrderRepository.Object
            );

            var result = await _service.CreateOrderAsync(new OrderRequest
            {
                Books = new List<RequestItem>
                    {
                        new RequestItem { BookId = "1234567890", Quantity = 10 }
                    }
            });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reason, Is.EqualTo("Not enough stock for book 1234567890"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task GetOrderById_ReturnsNullIfNotFound()
        {
            var result = await _service.GetOrderByIdAsync("nonexistent");
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reason, Is.EqualTo("Order not found"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task GetOrderById_ReturnsOrderIfFound()
        {
            _auditFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(_auditServiceMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IStockUpdateService))).Returns(_auditServiceMock.Object);

            _bookCatalogClient.Setup(c => c.GetBookByIdAsync("1234567890")).ReturnsAsync(new ResponseModel<Book?>
            {
                IsSuccess = true,
                Data = new Book("1234567890", "Test Book", "Test Author", 15.99m, 5)
            });

            _serviceProviderMock.Setup(x => x.GetService(typeof(Clients.IBookCatalogClient))).Returns(_bookCatalogClient.Object);

            var order = new Order
            {
                Id = "1234567890",
                Books = new List<OrderBook>
                {
                    new OrderBook { BookId = "1234567890", Quantity = 5, UnitPrice = 15.99m }
                }
            };

            _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(order);

            _service = new Services.OrderService(
                _auditFactoryMock.Object,
                _serviceProviderMock.Object,
                _stockUpdateService.Object,
                _bookCatalogClient.Object,
                _mockOrderRepository.Object
            );

            var result = await _service.GetOrderByIdAsync("1234567890");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.EqualTo("1234567890"));
            Assert.That(result.Data.Books.Count(), Is.EqualTo(1));
            Assert.That(result.Data.Books.First().BookId, Is.EqualTo("1234567890"));
            Assert.That(result.Data.Books.First().Quantity, Is.EqualTo(5));
            Assert.That(result.Data.Books.First().UnitPrice, Is.EqualTo(15.99m));
            Assert.That(result.Data.TotalPrice, Is.EqualTo(79.95m));
        }
    }
}