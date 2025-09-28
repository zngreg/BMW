namespace BMW.Books.OrderService.Services
{
    public class RabbitMqStockUpdateService : BaseRabbitMqService, IStockUpdateService
    {
        public RabbitMqStockUpdateService(IConfiguration config) : base(config)
        {
            _queue = config["RABBITMQ_STOCK_UPDATE_QUEUE"] ?? "stock-update-queue";
        }

        public async Task SendStockUpdateAsync(string message)
        {
            await SendRabbitMqMessage(message);
        }
    }
}
