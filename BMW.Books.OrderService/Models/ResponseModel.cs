namespace BMW.Books.OrderService.Models
{
    public struct ResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public string? Reason { get; set; }
        public T? Data { get; set; }
    }
}
