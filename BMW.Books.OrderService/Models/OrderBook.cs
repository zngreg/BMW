namespace BMW.Books.OrderService.Models
{
    public record OrderBook
    {
        public string BookId { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
