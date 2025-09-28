using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(string orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Dictionary<string, Order>> GetAllOrdersAsync();
    }
}