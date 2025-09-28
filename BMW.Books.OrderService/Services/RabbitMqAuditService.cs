namespace BMW.Books.OrderService.Services
{
    public class RabbitMqAuditService : RabbitMqStockUpdateService, IAuditService
    {
        public RabbitMqAuditService(IConfiguration config) : base(config)
        {
            _queue = config["RABBITMQ_QUEUE"] ?? "default-queue";
        }

        public async Task SendAuditAsync(string message)
        {
            await SendRabbitMqMessage($"[OrderService] {DateTime.UtcNow:o} {message}");
        }
    }
}
