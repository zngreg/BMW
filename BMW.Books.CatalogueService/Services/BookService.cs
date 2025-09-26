using BMW.Books.CatalogueService.Models;

namespace BMW.Books.CatalogueService.Services
{
    public class BookService : IBookService
    {
        private readonly Dictionary<string, Book> _books = new();
        private readonly IAuditService _auditService;

        public BookService(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<Book> AddBookAsync(BookRequest book)
        {
            var newBook = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = book.Title,
                Author = book.Author,
                Price = book.Price
            };
            _books[newBook.Id] = newBook;

            await _auditService.SendAuditAsync($"Book added: {newBook.Id} | {newBook.Title} by {newBook.Author}");
            return newBook;
        }

        public IEnumerable<Book> GetAllBooks() => _books.Values;

        public Book? GetBookById(string id) => _books.TryGetValue(id, out var b) ? b : null;
    }
}
