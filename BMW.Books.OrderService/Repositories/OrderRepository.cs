using BMW.Books.OrderService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BMW.Books.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "BOOK_STORE";

        public OrderRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Dictionary<string, Order> Orders => _cache.GetOrCreate(CacheKey, _ => new Dictionary<string, Order>());

        public Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return Task.FromResult(Orders.TryGetValue(orderId, out Order? order) ? order : null);
        }

        public Task<Order> CreateOrderAsync(Order order)
        {
            Orders[order.Id] = order;
            _cache.Set(order.Id, order);

            return Task.FromResult(order);
        }
    }
}