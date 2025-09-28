using BMW.Books.CatalogueService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BMW.Books.CatalogueService.Services
{
    public class RabbitMqStockUpdateListener : IStockUpdateListener
    {
        private readonly string _host;
        private readonly string _queue;
        private readonly string _user;
        private readonly string _pass;
        private readonly ILogger<RabbitMqStockUpdateListener> _log;
        private readonly IBookService _bookService;
        private IConnection? _conn;
        private IChannel? _ch;

        public RabbitMqStockUpdateListener(IConfiguration config, ILogger<RabbitMqStockUpdateListener> log, IBookService bookService)
        {
            _host = config["RABBITMQ_HOST"] ?? "rabbitmq";
            _queue = config["RABBITMQ_STOCK_UPDATE_QUEUE"] ?? "stock-update-queue";
            _user = config["RABBITMQ_USER"] ?? "guest";
            _pass = config["RABBITMQ_PASS"] ?? "guest";
            _log = log;
            _bookService = bookService;
        }

        public async Task StartAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory { HostName = _host, UserName = _user, Password = _pass };
                    _conn = await factory.CreateConnectionAsync(cancellationToken: token);
                    _ch = await _conn.CreateChannelAsync(cancellationToken: token);

                    await _ch.QueueDeclareAsync(
                        queue: _queue, durable: true, exclusive: false, autoDelete: false,
                        arguments: null, cancellationToken: token);

                    await _ch.BasicQosAsync(0, prefetchCount: 20, global: false, cancellationToken: token);

                    var consumer = new AsyncEventingBasicConsumer(_ch);
                    consumer.ReceivedAsync += async (_, ea) =>
                    {
                        try
                        {
                            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            _log.LogInformation("[BookService][RabbitMQ] {Message}", message);
                            Console.WriteLine($"DBUG => [BookService][RabbitMQ] {message}");
                            var stockUpdateMessage = JsonSerializer.Deserialize<StockUpdateMessage>(message);
                            if (stockUpdateMessage != null)
                            {
                                _ = _bookService.UpdateBookStockAsync(stockUpdateMessage.ISBN, stockUpdateMessage.StockChange);
                            }
                            await _ch!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: token);
                        }
                        catch (Exception ex)
                        {
                            _log.LogError(ex, "Failed to process message");

                            if (_ch is not null)
                                await _ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: token);
                        }
                    };

                    // Start consuming
                    await _ch.BasicConsumeAsync(
                        queue: _queue, autoAck: false, consumer: consumer, cancellationToken: token);

                    _log.LogInformation("RabbitMQ consumer started on {Host} queue {Queue}", _host, _queue);

                    while (!token.IsCancellationRequested && _conn.IsOpen && _ch.IsOpen)
                        await Task.Delay(1000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Consumer error. Will retry in 5 seconds.");
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
                finally
                {
                    try
                    {
                        if (_ch is not null) { await _ch.CloseAsync(token); _ch.Dispose(); }
                        if (_conn is not null) { await _conn.CloseAsync(token); _conn.Dispose(); }
                    }
                    catch { }

                    _ch = null;
                    _conn = null;
                }
            }
        }
    }
}