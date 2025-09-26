namespace BMW.Books.CatalogueService.Models
{
    public record BookRequest(string Title, string Author, decimal Price);
}