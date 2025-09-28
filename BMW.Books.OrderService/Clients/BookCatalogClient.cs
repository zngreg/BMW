using BMW.Books.OrderService.Models;

namespace BMW.Books.OrderService.Clients
{
    public class BookCatalogClient : IBookCatalogClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _bookServiceBase;
        private readonly HttpClient _httpClient;

        public BookCatalogClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _bookServiceBase = config["BOOKS_BASE_URL"] ?? "http://book-catalogue:8080";

            _httpClient = _httpClientFactory.CreateClient("books");
            _httpClient.BaseAddress = new Uri(_bookServiceBase);
        }
        public async Task<ResponseModel<Book?>> GetBookByIdAsync(string bookId)
        {
            return await _httpClient.GetFromJsonAsync<ResponseModel<Book?>>($"/books/{bookId}");
        }
    }
}
