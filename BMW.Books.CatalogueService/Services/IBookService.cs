using BMW.Books.CatalogueService.Models;

namespace BMW.Books.CatalogueService.Services
{
    public interface IBookService
    {
        Task<Book> AddBookAsync(BookRequest book);
        IEnumerable<Book> GetAllBooks();
        Book? GetBookById(string id);
    }
}
