using BMW.Books.CatalogueService.Models;

namespace BMW.Books.CatalogueService.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly Dictionary<string, Book> _books;
        public BookRepository()
        {
            _books ??= [];
        }

        public async Task<Book> AddOrUpdate(string isbn, Book book)
        {
            _books[isbn] = book;
            return await Task.FromResult(book);
        }

        public async Task<Book?> GetBookByIsbn(string isbn) => await Task.FromResult(_books.TryGetValue(isbn, out var book) ? book : null);

        public async Task<IEnumerable<Book>> GetAllBooks() => await Task.FromResult(_books.Values);

        public async Task<bool> Remove(string isbn) => await Task.FromResult(_books.Remove(isbn));

        public async Task Clear() => await Task.Run(() => _books.Clear());
    }
}
