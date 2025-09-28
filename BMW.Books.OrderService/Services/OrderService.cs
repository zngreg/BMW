using System.Globalization;
using System.Text.Json;
using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IStockUpdateService _stockUpdateService;
        private readonly Dictionary<string, Order> _orders = new();
        private readonly IAuditService _auditService;
        private readonly Clients.IBookCatalogClient _bookCatalogClient;

        public OrderService(
            IAuditServiceFactory auditFactory,
            IServiceProvider serviceProvider,
            IStockUpdateService stockUpdateService,
            Clients.IBookCatalogClient bookCatalogClient)
        {
            _stockUpdateService = stockUpdateService;
            _auditService = auditFactory.Create(serviceProvider);
            _bookCatalogClient = bookCatalogClient;
        }

        public async Task<ResponseModel<Order?>> CreateOrderAsync(OrderRequest req)
        {
            IEnumerable<OrderBook> orderItems = [];
            Dictionary<string, Book?> bookCache = new();

            foreach (var item in req.Books)
            {
                var validationResult = await ValidateOrderItem(item);
                if (!validationResult.IsSuccess)
                {
                    await _auditService.SendAuditAsync($"Order item validation failed for BookId: {item.BookId}, Reason: {validationResult.Reason}");
                    return new ResponseModel<Order?> { IsSuccess = false, Reason = validationResult.Reason };
                }

                bookCache[item.BookId] = validationResult.Data;

                orderItems = orderItems.Append(new OrderBook
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = validationResult.Data?.Price ?? 0
                });
            }

            var order = new Order
            {
                Id = Guid.NewGuid().ToString("N"),
                Books = orderItems,
                CreatedAtUtc = DateTime.UtcNow
            };
            _orders[order.Id] = order;

            foreach (var item in order.Books)
            {
                await _stockUpdateService.SendStockUpdateAsync(JsonSerializer.Serialize(new StockUpdateMessage { ISBN = item.BookId, StockChange = item.Quantity }));
            }

            await _auditService.SendAuditAsync($"Order placed: {order.Id}\nItems:\n{string.Join("\n", order.Books.Select(b => $"* {b.BookId} | {bookCache[b.BookId]?.Title} by {bookCache[b.BookId]?.Author} x{b.Quantity} @ {b.UnitPrice.ToString("C", CultureInfo.CurrentCulture)}"))}\nTotal: {order.TotalPrice.ToString("C", CultureInfo.CurrentCulture)}");
            return new ResponseModel<Order?> { IsSuccess = true, Data = order };
        }

        public async Task<ResponseModel<Order?>> GetOrderByIdAsync(string id)
        {
            return await Task.FromResult(_orders.TryGetValue(id, out var outputOrder) ? new ResponseModel<Order?> { IsSuccess = true, Data = outputOrder } : new ResponseModel<Order?> { IsSuccess = false, Reason = "Order not found" });
        }

        private async Task<ResponseModel<Book?>> ValidateOrderItem(RequestItem item)
        {
            if (item.Quantity <= 0)
                return new ResponseModel<Book?> { IsSuccess = false, Reason = "Quantity must be greater than zero" };

            var book = await _bookCatalogClient.GetBookByIdAsync(item.BookId);
            if (!book.IsSuccess || book.Data is null)
                return new ResponseModel<Book?> { IsSuccess = false, Reason = $"Book with ID {item.BookId} not found" };

            if (item.Quantity > book.Data.Stock)
                return new ResponseModel<Book?> { IsSuccess = false, Reason = $"Not enough stock for book {item.BookId}" };

            return new ResponseModel<Book?>
            {
                IsSuccess = true,
                Data = book.Data
            };
        }
    }
}
