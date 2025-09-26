using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<string, Order> _orders = new();
        private readonly string _bookServiceBase;
        private readonly IAuditService _auditService;

        public OrderService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IAuditService auditService)
        {
            _httpClientFactory = httpClientFactory;
            _bookServiceBase = config["BOOKS_BASE_URL"] ?? "http://book-catalogue:8080";
            _auditService = auditService;
        }

        public async Task<Order?> CreateOrderAsync(OrderRequest req)
        {
            var client = _httpClientFactory.CreateClient("books");
            var book = await client.GetFromJsonAsync<Book>($"/books/{req.BookId}");
            if (book is null)
                return null;

            var order = new Order
            {
                Id = Guid.NewGuid().ToString("N"),
                BookId = req.BookId,
                Quantity = req.Quantity <= 0 ? 1 : req.Quantity,
                UnitPrice = book.Price,
                CreatedAtUtc = DateTime.UtcNow
            };
            _orders[order.Id] = order;
            await _auditService.SendAuditAsync($"Order placed: {order.Id} | Book {order.BookId} x{order.Quantity} @ {order.UnitPrice}");
            return order;
        }

        public Order? GetOrderById(string id)
        {
            return _orders.TryGetValue(id, out var o) ? o : null;
        }
    }
}
