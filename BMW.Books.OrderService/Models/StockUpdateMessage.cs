namespace BMW.Books.OrderService.Models
{
    public class StockUpdateMessage
    {
        public string ISBN { get; set; } = string.Empty;
        public int StockChange { get; set; }
    }
}