namespace BMW.Books.OrderService.Services
{
    public interface IAuditService
    {
        Task SendAuditAsync(string message);
    }
}
