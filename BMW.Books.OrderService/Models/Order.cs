namespace BMW.Books.OrderService.Models
{
    public record Order
    {
        public string Id { get; set; } = "";
        public IEnumerable<OrderBook>? Books { get; set; }
        public decimal TotalPrice => Books?.Sum(b => b.UnitPrice * b.Quantity) ?? 0;
        public DateTime CreatedAtUtc { get; set; }
    }
}