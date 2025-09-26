using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Services
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(OrderRequest req);
        Order? GetOrderById(string id);
    }
}
