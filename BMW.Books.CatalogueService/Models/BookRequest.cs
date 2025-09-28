using System.ComponentModel.DataAnnotations;

namespace BMW.Books.CatalogueService.Models
{
    public class BookRequest(string isbn, string title, string author, int stock, decimal price)
    {
        [Required, RegularExpression(@"^\d{3}-\d{1}-\d{3}-\d{5}-\d{1}$", ErrorMessage = "ISBN must be in the format 000-0-000-00000-0")]
        public string ISBN { get; set; } = isbn;
        [Required]
        public string Title { get; set; } = title;
        [Required]
        public string Author { get; set; } = author;
        [Required, Range(0.0, double.MaxValue)]
        public decimal Price { get; set; } = price;
        [Required, Range(1, int.MaxValue)]
        public int Stock { get; set; } = stock;
    }
}