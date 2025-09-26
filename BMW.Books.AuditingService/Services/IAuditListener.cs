namespace BMW.AuditingService.Services
{
    public interface IAuditListener
    {
        Task StartAsync(CancellationToken token);
    }
}
