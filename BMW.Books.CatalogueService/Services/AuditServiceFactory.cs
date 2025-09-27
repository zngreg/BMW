namespace BMW.Books.CatalogueService.Services
{
    public interface IAuditServiceFactory
    {
        IAuditService Create(IServiceProvider serviceProvider);
    }

    public class AuditServiceFactory : IAuditServiceFactory
    {
        private readonly IConfiguration _config;
        public AuditServiceFactory(IConfiguration config)
        {
            _config = config;
        }

        public IAuditService Create(IServiceProvider serviceProvider)
        {
            var type = _config["AUDIT_TYPE"]?.ToLower() ?? "udp";
            return type switch
            {
                "queue" => serviceProvider.GetRequiredService<RabbitMqAuditService>(),
                _ => serviceProvider.GetRequiredService<UdpAuditService>()
            };
        }
    }
}
