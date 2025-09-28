using BMW.Books.CatalogueService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    [TestFixture]
    public class AuditServiceFactoryTests
    {
        [Test]
        public void Create_ReturnsUdpAuditService_When_Type_Is_Udp_Should_Return_UdpAuditService()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_TYPE"]).Returns("udp");
            var provider = new ServiceProviderStub(typeof(UdpAuditService));
            var factory = new AuditServiceFactory(configMock.Object);
            var result = factory.Create(provider);
            Assert.That(result, Is.InstanceOf<UdpAuditService>());
        }

        [Test]
        public void Create_ReturnsUdpAuditService_When_Type_Is_Missing_Should_Return_UdpAuditService()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_TYPE"]).Returns((string)null);
            var provider = new ServiceProviderStub(typeof(UdpAuditService));
            var factory = new AuditServiceFactory(configMock.Object);
            var result = factory.Create(provider);
            Assert.That(result, Is.InstanceOf<UdpAuditService>());
        }

        [Test]
        public void Create_ReturnsRabbitMqAuditService_When_Type_Is_Queue_Should_Return_RabbitMqAuditService()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["AUDIT_TYPE"]).Returns("queue");
            var provider = new ServiceProviderStub(typeof(RabbitMqAuditService));
            var factory = new AuditServiceFactory(configMock.Object);
            var result = factory.Create(provider);
            Assert.That(result, Is.InstanceOf<RabbitMqAuditService>());
        }



        private class ServiceProviderStub : IServiceProvider
        {
            private readonly Type _type;
            public ServiceProviderStub(Type type) { _type = type; }
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(UdpAuditService)) return new UdpAuditService(new Mock<IConfiguration>().Object);
                if (serviceType == typeof(RabbitMqAuditService)) return new RabbitMqAuditService(new Mock<IConfiguration>().Object);
                throw new NotImplementedException();
            }
        }
    }
}
