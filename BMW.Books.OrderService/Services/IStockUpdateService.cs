namespace BMW.Books.OrderService.Services
{
    public interface IStockUpdateService
    {
        Task SendStockUpdateAsync(string message);
    }
}
