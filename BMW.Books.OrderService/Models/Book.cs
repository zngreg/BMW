namespace BMW.Books.OrderService.Models
{
    public record Book(string ISBN, string Title, string Author, decimal Price, int Stock);
}