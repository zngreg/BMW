using System.ComponentModel.DataAnnotations;

namespace BMW.Books.CatalogueService.Models
{
    public class BookRequest
    {
        [Required, RegularExpression(@"^\d{3}-\d{1}-\d{3}-\d{5}-\d{1}$", ErrorMessage = "ISBN must be in the format 000-0-000-00000-0")]
        public string ISBN { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required, Range(0.0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Stock { get; set; }
    }
}