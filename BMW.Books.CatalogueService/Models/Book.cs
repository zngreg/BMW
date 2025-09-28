namespace BMW.Books.CatalogueService.Models
{
    public record Book
    {
        public string ISBN { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
