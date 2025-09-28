using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Services
{
    public interface IOrderService
    {
        Task<ResponseModel<Order?>> CreateOrderAsync(OrderRequest req);
        Task<ResponseModel<Order?>> GetOrderByIdAsync(string id);
        Task<ResponseModel<IEnumerable<Order>>> GetAllOrdersAsnyc();
    }
}
