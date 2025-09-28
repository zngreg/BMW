using BMW.Books.CatalogueService.Models;

namespace BMW.Books.CatalogueService.Repositories
{
    public interface IBookRepository
    {
        Task<Book> AddOrUpdate(string isbn, Book book);

        Task<Book?> GetBookByIsbn(string isbn);

        Task<IEnumerable<Book>> GetAllBooks();

        Task<bool> Remove(string isbn);

        Task Clear();
    }
}
