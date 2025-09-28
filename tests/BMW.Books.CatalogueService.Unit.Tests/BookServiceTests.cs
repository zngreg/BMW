using BMW.Books.CatalogueService.Services;
using BMW.Books.CatalogueService.Models;
using Moq;
using BMW.Books.CatalogueService.Repositories;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<IAuditServiceFactory> _auditServiceFactoryMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IBookRepository> _bookRepositoryMock;
        private IBookService _service;

        [SetUp]
        public void SetUp()
        {
            _auditServiceFactoryMock = new Mock<IAuditServiceFactory>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);
        }

        [Test]
        public async Task AddBook_With_ValidData_Should_AddsAndReturnsBook()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            var bookRequest = new BookRequest(book.ISBN, book.Title, book.Author, book.Stock, book.Price);

            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _bookRepositoryMock.Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Book>())).ReturnsAsync(book);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.AddBookAsync(bookRequest);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo(book.ISBN));
            Assert.That(result.Data.Author, Is.EqualTo(book.Author));
            Assert.That(result.Data.Title, Is.EqualTo(book.Title));
            Assert.That(result.Data.Price, Is.EqualTo(book.Price));
            Assert.That(result.Data.Stock, Is.EqualTo(book.Stock));
        }

        [Test]
        public async Task AddBook_With_ExistingData_And_Different_Title_Should_ReturnsFailure()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            var bookRequest = new BookRequest(book.ISBN, "Title B", book.Author, book.Stock, book.Price);

            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.AddBookAsync(bookRequest);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo($"Book with same ISBN {book.ISBN} but different details already exists. Override not allowed."));
        }

        [Test]
        public async Task AddBook_With_ExistingData_And_Different_Author_Should_ReturnsFailure()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            var bookRequest = new BookRequest(book.ISBN, book.Title, "Author B", book.Stock, book.Price);

            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.AddBookAsync(bookRequest);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo($"Book with same ISBN {book.ISBN} but different details already exists. Override not allowed."));
        }

        [Test]
        public async Task AddBook_With_ExistingData_Should_UpdateStock()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            var bookRequest = new BookRequest(book.ISBN, book.Title, book.Author, 10, book.Price);

            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Book>())).ReturnsAsync(book);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.AddBookAsync(bookRequest);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo(book.ISBN));
            Assert.That(result.Data.Author, Is.EqualTo(book.Author));
            Assert.That(result.Data.Title, Is.EqualTo(book.Title));
            Assert.That(result.Data.Price, Is.EqualTo(book.Price));
            Assert.That(result.Data.Stock, Is.EqualTo(15));
        }

        [Test]
        public async Task GetBookById_Should_ReturnsCorrectBookAsync()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.GetBookByISBNAsync(book.ISBN);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo(book.ISBN));
            Assert.That(result.Data.Author, Is.EqualTo(book.Author));
            Assert.That(result.Data.Title, Is.EqualTo(book.Title));
            Assert.That(result.Data.Price, Is.EqualTo(book.Price));
        }

        [Test]
        public async Task GetBookById_Should_ReturnsNullIfNotFound()
        {
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.GetBookByISBNAsync("not-exist");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo("Book not found"));
        }

        [Test]
        public async Task GetAllBooks_With_ExistingBooks_Should_ReturnsAllBooksAsync()
        {
            var books = new List<Book>
            {
                new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M },
                new Book { ISBN = "111-1-111-11111-1", Title = "Title B", Author = "Author B", Stock = 3, Price = 15.0M }
            };
            _bookRepositoryMock.Setup(r => r.GetAllBooks()).ReturnsAsync(books);
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);


            var result = await _service.GetAllBooksAsync();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllBooks_With_Non_ExistingBooks_Should_ReturnsEmptyListAsync()
        {
            var books = new List<Book>();
            _bookRepositoryMock.Setup(r => r.GetAllBooks()).ReturnsAsync(books);
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);


            var result = await _service.GetAllBooksAsync();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateBook_With_ValidData_Should_UpdatesAndReturnsBookAsync()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 5, Price = 20.5M };
            var updatedBookRequest = new BookRequest(book.ISBN, "Title B", "Author B", 10, 25.0M);
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Book>())).ReturnsAsync(book);
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.UpdateBookAsync(book.ISBN, updatedBookRequest);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo(book.ISBN));
            Assert.That(result.Data.Author, Is.EqualTo(updatedBookRequest.Author));
            Assert.That(result.Data.Title, Is.EqualTo(updatedBookRequest.Title));
            Assert.That(result.Data.Price, Is.EqualTo(updatedBookRequest.Price));
            Assert.That(result.Data.Stock, Is.EqualTo(updatedBookRequest.Stock));
        }

        [Test]
        public async Task UpdateBook_With_InvalidData_Should_ReturnsNotFoundAsync()
        {
            var updatedBookRequest = new BookRequest("NON_EXISTING_ISBN", "Title B", "Author B", 10, 25.0M);
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.UpdateBookAsync(updatedBookRequest.ISBN, updatedBookRequest);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo("Book not found"));
        }

        [Test]
        public async Task UpdateBookStockAsync_With_Valid_ISBN_Should_UpdatesStock()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 10, Price = 20.5M };
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Book>())).ReturnsAsync(book);
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.UpdateBookStockAsync(book.ISBN, 5);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo(book.ISBN));
            Assert.That(result.Data.Author, Is.EqualTo(book.Author));
            Assert.That(result.Data.Title, Is.EqualTo(book.Title));
            Assert.That(result.Data.Price, Is.EqualTo(book.Price));
            Assert.That(result.Data.Stock, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateBookStockAsync_With_InvalidData_Should_ReturnsNotFoundAsync()
        {
            var updatedBookRequest = new BookRequest("NON_EXISTING_ISBN", "Title B", "Author B", 10, 25.0M);
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.UpdateBookStockAsync(updatedBookRequest.ISBN, 5);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo("Book not found"));
        }

        [Test]
        public async Task UpdateBookStockAsync_With_Update_Quantity_More_Than_Existing_Should_ReturnError()
        {
            var book = new Book { ISBN = "000-0-000-00000-0", Title = "Title A", Author = "Author A", Stock = 10, Price = 20.5M };
            _bookRepositoryMock.Setup(r => r.GetBookByIsbn(It.IsAny<string>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.AddOrUpdate(It.IsAny<string>(), It.IsAny<Book>())).ReturnsAsync(book);
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);

            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object, _bookRepositoryMock.Object);

            var result = await _service.UpdateBookStockAsync(book.ISBN, 12);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Reason, Is.EqualTo("Insufficient stock"));
        }
    }
}