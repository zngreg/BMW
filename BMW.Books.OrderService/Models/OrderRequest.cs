using System.ComponentModel.DataAnnotations;

namespace BMW.Books.OrderService.Models
{
    public class OrderRequest
    {
        [Required, MinLength(1, ErrorMessage = "At least one book is required")]
        public List<RequestItem> Books { get; set; } = new();
    }
}