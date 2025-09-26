namespace BMW.Books.CatalogueService.Models
{
    public record Book
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public decimal Price { get; set; }
    }
}
