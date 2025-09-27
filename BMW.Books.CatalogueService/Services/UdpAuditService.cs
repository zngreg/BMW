using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BMW.Books.CatalogueService.Services
{
    public class UdpAuditService : IAuditService
    {
        private readonly string _auditingHost;
        private readonly int _auditingPort;

        public UdpAuditService(IConfiguration config)
        {
            _auditingHost = config["AUDIT_HOST"] ?? "auditing-service";
            _auditingPort = int.TryParse(config["AUDIT_PORT"], out var p) ? p : 5140;
        }

        public async Task SendAuditAsync(string message)
        {
            using var client = new UdpClient();
            var data = Encoding.UTF8.GetBytes($"[BookCatalogue] {DateTime.UtcNow:o} {message}");
            var ip = IPAddress.Parse(await ResolveHost(_auditingHost));
            await client.SendAsync(data, data.Length, new IPEndPoint(ip, _auditingPort));
        }

        private async Task<string> ResolveHost(string host)
        {
            var entry = await Dns.GetHostEntryAsync(host);
            return entry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
