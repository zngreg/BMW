namespace BMW.Books.OrderService.Models
{
    public record OrderRequest(string BookId, int Quantity);
}