using RabbitMQ.Client;
using System.Text;

namespace BMW.Books.CatalogueService.Services
{
    public class RabbitMqAuditService : IAuditService
    {
        private readonly string _host;
        private readonly string _queue;
        private readonly string _user;
        private readonly string _pass;

        public RabbitMqAuditService(IConfiguration config)
        {
            _host = config["RABBITMQ_HOST"] ?? "localhost";
            _queue = config["RABBITMQ_QUEUE"] ?? "audit-queue";
            _user = config["RABBITMQ_USER"] ?? "guest";
            _pass = config["RABBITMQ_PASS"] ?? "guest";
        }

        public async Task SendAuditAsync(string message)
        {
            var factory = new ConnectionFactory { HostName = _host, UserName = _user, Password = _pass };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes($"[BookCatalogue] {DateTime.UtcNow:o} {message}");

            var props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                ContentType = "text/plain"
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queue,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }
    }
}
