namespace BMW.Books.AuditingService.Services
{
    public interface IAuditListener
    {
        Task StartAsync(CancellationToken token);
    }
}
