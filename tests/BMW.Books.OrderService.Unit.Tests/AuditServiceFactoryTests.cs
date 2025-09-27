using System;
using BMW.Books.OrderService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class AuditServiceFactoryTests
    {
        // [Test]
        // public void Create_ReturnsUdpAuditService_WhenTypeIsUdpOrNull()
        // {
        //     var configMock = new Mock<IConfiguration>();
        //     configMock.Setup(c => c["AUDIT_TYPE"]).Returns((string)null);
        //     var factory = new AuditServiceFactory(configMock.Object);
        //     var spMock = new Mock<IServiceProvider>();
        //     spMock.Setup(s => s.GetService(typeof(UdpAuditService))).Returns(new Mock<IAuditService>().Object);
        //     var result = factory.Create(spMock.Object);
        //     Assert.That(result, Is.Not.Null);
        // }

        // [Test]
        // public void Create_ReturnsRabbitMqAuditService_WhenTypeIsQueue()
        // {
        //     var configMock = new Mock<IConfiguration>();
        //     configMock.Setup(c => c["AUDIT_TYPE"]).Returns("queue");
        //     var factory = new AuditServiceFactory(configMock.Object);
        //     var spMock = new Mock<IServiceProvider>();
        //     spMock.Setup(s => s.GetService(typeof(RabbitMqAuditService))).Returns(new Mock<IAuditService>().Object);
        //     var result = factory.Create(spMock.Object);
        //     Assert.That(result, Is.Not.Null);
        // }
    }
}
