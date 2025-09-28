using System.ComponentModel.DataAnnotations;

namespace BMW.Books.OrderService.Models
{
    public class RequestItem
    {
        [Required, RegularExpression(@"^\d{3}-\d{1}-\d{3}-\d{5}-\d{1}$", ErrorMessage = "BookId (ISBN) must be in the format 000-0-000-00000-0")]
        public string BookId { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class OrderRequest
    {
        [Required, MinLength(1, ErrorMessage = "At least one book is required")]
        public List<RequestItem> Books { get; set; } = new();
    }
}