using BMW.Books.CatalogueService.Services;
using BMW.Books.CatalogueService.Models;
using Moq;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<IAuditServiceFactory> _auditServiceFactoryMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private IBookService _service;

        [SetUp]
        public void SetUp()
        {
            _auditServiceFactoryMock = new Mock<IAuditServiceFactory>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object);
        }

        [Test]
        public async Task AddBook_AddsAndReturnsBook()
        {
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object);

            var book = new BookRequest("Test", "A", 10M);
            var result = await _service.AddBookAsync(book);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Author, Is.EqualTo(book.Author));
            Assert.That(result.Title, Is.EqualTo(book.Title));
            Assert.That(result.Price, Is.EqualTo(book.Price));

            Assert.That(_service.GetAllBooks().Contains(result));
        }

        [Test]
        public async Task GetBookById_ReturnsCorrectBookAsync()
        {
            _auditServiceFactoryMock.Setup(f => f.Create(It.IsAny<IServiceProvider>())).Returns(new Mock<IAuditService>().Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuditService))).Returns(new Mock<IAuditService>().Object);
            _service = new BookService(_auditServiceFactoryMock.Object, _serviceProviderMock.Object);

            var book = new BookRequest("Test", "A", 10M);
            var result = await _service.AddBookAsync(book);
            var found = _service.GetBookById(result.Id);

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Id, Is.EqualTo(result.Id));
            Assert.That(found.Title, Is.EqualTo(result.Title));
            Assert.That(found.Author, Is.EqualTo(result.Author));
            Assert.That(found.Price, Is.EqualTo(result.Price));
        }

        [Test]
        public void GetBookById_ReturnsNullIfNotFound()
        {
            var found = _service.GetBookById("not-exist");
            Assert.That(found, Is.Null);
        }
    }
}
