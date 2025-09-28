using System.Text;
using RabbitMQ.Client;

namespace BMW.Books.OrderService.Services
{
    public class BaseRabbitMqService
    {
        protected string _host;
        protected string _queue;
        protected string _user;
        protected string _pass;

        public BaseRabbitMqService(IConfiguration config)
        {
            _host = config["RABBITMQ_HOST"] ?? "localhost";
            _user = config["RABBITMQ_USER"] ?? "guest";
            _pass = config["RABBITMQ_PASS"] ?? "guest";
        }

        public async Task SendRabbitMqMessage(string message)
        {
            var factory = new ConnectionFactory { HostName = _host, UserName = _user, Password = _pass };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(_queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);

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
