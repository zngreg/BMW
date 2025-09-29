using BMW.Books.CatalogueService.Models;
using BMW.Books.CatalogueService.Repositories;

namespace BMW.Books.CatalogueService.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuditService _auditService;

        public BookService(
            IAuditServiceFactory auditServiceFactory,
            IServiceProvider serviceProvider,
            IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _auditService = auditServiceFactory.Create(serviceProvider);
        }

        public async Task<ResponseModel<Book>> AddBookAsync(BookRequest book)
        {
            if (await _bookRepository.GetBookByIsbn(book.ISBN) is Book value)
            {
                if (!value.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase) || !value.Author.Equals(book.Author, StringComparison.OrdinalIgnoreCase))
                {
                    await _auditService.SendAuditAsync($"Book with same ISBN {book.ISBN} but different details already exists. Override not allowed.");
                    return new ResponseModel<Book> { IsSuccess = false, Reason = $"Book with same ISBN {book.ISBN} but different details already exists. Override not allowed." };
                }

                book.Stock += value.Stock;
            }

            var newBook = new Book
            {
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Stock = book.Stock
            };
            await _bookRepository.AddOrUpdate(newBook.ISBN, newBook);

            await _auditService.SendAuditAsync($"Book added: {newBook.ISBN} | {newBook.Title} by {newBook.Author} Quantity: {newBook.Stock} @ {newBook.Price:R0.00}");
            return new ResponseModel<Book> { IsSuccess = true, Data = newBook };
        }

        public async Task<ResponseModel<IEnumerable<Book?>>> GetAllBooksAsync() => await Task.FromResult(new ResponseModel<IEnumerable<Book?>> { IsSuccess = true, Data = await _bookRepository.GetAllBooks() ?? [] });

        public async Task<ResponseModel<Book?>> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepository.GetBookByIsbn(isbn);
            return book is not null
                ? new ResponseModel<Book?> { IsSuccess = true, Data = book }
                : new ResponseModel<Book?> { IsSuccess = false, Reason = "Book not found" };
        }

        public async Task<ResponseModel<Book?>> UpdateBookAsync(string isbn, BookRequest bookRequest)
        {

            if (await _bookRepository.GetBookByIsbn(isbn) is not Book book)
            {
                await _auditService.SendAuditAsync($"Book not found: {isbn}");
                return new ResponseModel<Book?> { IsSuccess = false, Reason = "Book not found" };
            }

            book.Title = bookRequest.Title;
            book.Author = bookRequest.Author;
            book.Price = bookRequest.Price;
            book.Stock = bookRequest.Stock;
            await _bookRepository.AddOrUpdate(isbn, book);

            await _auditService.SendAuditAsync($"Book updated: {book.ISBN} | {book.Title} by {book.Author} Quantity: {book.Stock} @ {book.Price:R0.00}");

            return new ResponseModel<Book?> { IsSuccess = true, Data = book };
        }

        public async Task<ResponseModel<Book?>> UpdateBookStockAsync(string isbn, int stockChange)
        {
            if (await _bookRepository.GetBookByIsbn(isbn) is not Book book)
            {
                await _auditService.SendAuditAsync($"Book not found: {isbn}");
                return new ResponseModel<Book?> { IsSuccess = false, Reason = "Book not found" };
            }

            if (book.Stock - stockChange < 0)
            {
                await _auditService.SendAuditAsync($"Insufficient stock for: {book.ISBN} | {book.Title} by {book.Author} Quantity: {book.Stock}");
                return new ResponseModel<Book?> { IsSuccess = false, Reason = "Insufficient stock" };
            }

            book.Stock -= stockChange;
            await _bookRepository.AddOrUpdate(isbn, book);

            await _auditService.SendAuditAsync($"Book stock updated: {book.ISBN} | {book.Title} by {book.Author} Quantity: {book.Stock} @ {book.Price:R0.00}");

            return new ResponseModel<Book?> { IsSuccess = true, Data = book };
        }
    }
}
