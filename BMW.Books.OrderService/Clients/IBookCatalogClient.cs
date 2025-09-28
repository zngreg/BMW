using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Clients
{
    public interface IBookCatalogClient
    {
        Task<ResponseModel<Book?>> GetBookByIdAsync(string bookId);
    }
}
