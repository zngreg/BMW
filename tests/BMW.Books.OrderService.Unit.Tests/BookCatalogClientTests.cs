using System.Net;
using System.Net.Http.Json;
using BMW.Books.OrderService.Clients;
using BMW.Books.OrderService.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace BMW.Books.OrderService.Unit.Tests
{
    public class BookCatalogClientTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IConfiguration> _configMock;
        private HttpClient _httpClient;
        private BookCatalogClient _client;
        private Mock<HttpMessageHandler> _handlerMock;

        [SetUp]
        public void Setup()
        {
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _handlerMock.Protected().Setup("Dispose", ItExpr.IsAny<bool>());
            _httpClient = new HttpClient(_handlerMock.Object);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(f => f.CreateClient("books")).Returns(_httpClient);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["BOOKS_BASE_URL"]).Returns("http://fake-url");
            _client = new BookCatalogClient(_httpClientFactoryMock.Object, _configMock.Object);
        }

        [Test]
        public async Task GetBookByIdAsync_ReturnsBook()
        {
            var expectedBook = new Book("1", "Test", "Author", 9.99m, 5);
            var expected = new ResponseModel<Book?> { IsSuccess = true, Data = expectedBook };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expected)
            };
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            var result = await _client.GetBookByIdAsync("1");
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.ISBN, Is.EqualTo("1"));
            Assert.That(result.Data.Title, Is.EqualTo("Test"));
        }
        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }
    }
}
