using System.Net.Sockets;
using System.Text;

namespace BMW.AuditingService.Services
{
    public class AuditListener : IAuditListener
    {
        private readonly int _port;

        public AuditListener(IConfiguration config)
        {
            _port = int.TryParse(config["AUDIT_PORT"], out var p) ? p : 5140;
        }

        public async Task StartAsync(CancellationToken token)
        {
            using var udp = new UdpClient(_port);
            Console.WriteLine($"[AuditingService] Listening on UDP {_port}");
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await udp.ReceiveAsync(token);
                    var msg = Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"[AUDIT] {DateTime.UtcNow:o} {msg}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuditingService] Error: {ex.Message}");
                }
            }
        }
    }
}
