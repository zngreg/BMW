using BMW.Books.OrderService.Models;
using BMW.Books.OrderService.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BMW.Books.OrderService.Unit.Tests
{
    public class OrderRepositoryTests
    {
        private OrderRepository _repo;
        private MemoryCache _memoryCache;
        private Order _order;

        [SetUp]
        public void Setup()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _repo = new OrderRepository(_memoryCache);
            _order = new Order
            {
                Id = "order1",
                Books = new List<OrderBook> { new OrderBook { BookId = "b1", Quantity = 2, UnitPrice = 10 } },
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Dispose();
        }

        [Test]
        public async Task CreateOrderAsync_AddsOrder()
        {
            var result = await _repo.CreateOrderAsync(_order);

            Assert.That(result, Is.Not.Null);
            Assert.That(_order, Is.EqualTo(result));
            Assert.That(_order.Id, Is.EqualTo(result.Id));
            Assert.That(_order.TotalPrice, Is.EqualTo(result.TotalPrice));
            Assert.That(_order.CreatedAtUtc, Is.EqualTo(result.CreatedAtUtc));
            Assert.That(_order.Books.Count, Is.EqualTo(result.Books.Count()));
            Assert.That(_repo.Orders.ContainsKey(_order.Id));
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenExists()
        {
            var orders = _repo.Orders;
            orders[_order.Id] = _order;

            var result = await _repo.GetOrderByIdAsync(_order.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(_order, Is.EqualTo(result));
            Assert.That(_order.Id, Is.EqualTo(result.Id));
            Assert.That(_order.TotalPrice, Is.EqualTo(result.TotalPrice));
            Assert.That(_order.CreatedAtUtc, Is.EqualTo(result.CreatedAtUtc));
            Assert.That(_order.Books.Count, Is.EqualTo(result.Books.Count()));
            Assert.That(orders.ContainsKey(_order.Id));
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenNotExists()
        {
            var result = await _repo.GetOrderByIdAsync("notfound");

            Assert.That(result, Is.Null);
        }
    }
}
