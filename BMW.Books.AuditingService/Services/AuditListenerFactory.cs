namespace BMW.Books.AuditingService.Services
{
    public interface IAuditListenerFactory
    {
        IAuditListener CreateListener(IServiceProvider serviceProvider);
    }

    public class AuditListenerFactory : IAuditListenerFactory
    {
        private readonly IConfiguration _config;
        public AuditListenerFactory(IConfiguration config)
        {
            _config = config;
        }

        public IAuditListener CreateListener(IServiceProvider serviceProvider)
        {
            var type = _config["AUDIT_TYPE"]?.ToLower() ?? "udp";
            return type switch
            {
                "queue" => serviceProvider.GetRequiredService<RabbitMqAuditListener>(),
                _ => serviceProvider.GetRequiredService<UdpAuditListener>()
            };
        }
    }
}
