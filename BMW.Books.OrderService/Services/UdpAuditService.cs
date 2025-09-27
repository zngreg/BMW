using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BMW.Books.OrderService.Services
{
    public class UdpAuditService : IAuditService
    {
        private readonly string _host;
        private readonly int _port;

        public UdpAuditService(IConfiguration config)
        {
            _host = config["AUDIT_HOST"] ?? "auditing-service";
            _port = int.TryParse(config["AUDIT_PORT"], out var p) ? p : 5140;
        }

        public async Task SendAuditAsync(string message)
        {
            using var client = new UdpClient();
            var data = Encoding.UTF8.GetBytes($"[OrderService] {DateTime.UtcNow:o} {message}");
            var ip = IPAddress.Parse(await ResolveHost(_host));
            await client.SendAsync(data, data.Length, new IPEndPoint(ip, _port));
        }

        private async Task<string> ResolveHost(string host)
        {
            var entry = await Dns.GetHostEntryAsync(host);
            return entry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
