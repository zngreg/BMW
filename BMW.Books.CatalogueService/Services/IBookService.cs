using BMW.Books.CatalogueService.Models;

namespace BMW.Books.CatalogueService.Services
{
    public interface IBookService
    {
        Task<ResponseModel<Book>> AddBookAsync(BookRequest book);
        Task<ResponseModel<IEnumerable<Book?>>> GetAllBooksAsync();
        Task<ResponseModel<Book?>> GetBookByISBNAsync(string isbn);
        Task<ResponseModel<Book?>> UpdateBookAsync(string isbn, BookRequest book);
        Task<ResponseModel<Book?>> UpdateBookStockAsync(string isbn, int stockChange);
    }
}
