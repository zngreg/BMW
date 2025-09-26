namespace BMW.Books.CatalogueService.Services
{
    public interface IAuditService
    {
        Task SendAuditAsync(string message);
    }
}
