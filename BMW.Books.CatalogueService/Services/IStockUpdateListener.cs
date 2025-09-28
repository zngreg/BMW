namespace BMW.Books.CatalogueService.Services
{
    public interface IStockUpdateListener
    {
        Task StartAsync(CancellationToken token);
    }
}
