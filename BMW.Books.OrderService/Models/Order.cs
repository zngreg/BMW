namespace BMW.Books.OrderService.Models
{
    public record Order
    {
        public string Id { get; set; } = "";
        public string BookId { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
