using BMW.Books.CatalogueService.Models;
using BMW.Books.CatalogueService.Repositories;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    public class BookRepositoryTests
    {
        private BookRepository _repo;
        private Book _book;
        private string _isbn;

        [SetUp]
        public void Setup()
        {
            _repo = new BookRepository();
            _isbn = "1234567890";
            _book = new Book { ISBN = _isbn, Title = "Test Title", Author = "Test Author", Price = 9.99m, Stock = 10 };
        }

        [Test]
        public async Task AddOrUpdate_AddsBook()
        {
            var result = await _repo.AddOrUpdate(_isbn, _book);

            Assert.That(_book, Is.EqualTo(result));
            var fetched = await _repo.GetBookByIsbn(_isbn);
            Assert.That(_book, Is.EqualTo(fetched));
        }

        [Test]
        public async Task GetBookByIsbn_ReturnsNull_WhenNotFound()
        {
            var result = await _repo.GetBookByIsbn("notfound");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            await _repo.AddOrUpdate(_isbn, _book);
            var books = await _repo.GetAllBooks();
            Assert.That(books, Does.Contain(_book));
        }

        [Test]
        public async Task Remove_RemovesBook()
        {
            await _repo.AddOrUpdate(_isbn, _book);
            var removed = await _repo.Remove(_isbn);
            Assert.That(removed, Is.True);
            var result = await _repo.GetBookByIsbn(_isbn);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Clear_RemovesAllBooks()
        {
            await _repo.AddOrUpdate(_isbn, _book);
            await _repo.Clear();
            var books = await _repo.GetAllBooks();
            Assert.That(books, Is.Empty);
        }
    }
}
