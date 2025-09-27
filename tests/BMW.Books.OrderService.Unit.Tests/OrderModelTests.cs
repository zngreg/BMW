using System;
using BMW.Books.OrderService.Models;
using NUnit.Framework;

namespace BMW.Books.OrderService.Unit.Tests
{
    [TestFixture]
    public class OrderModelTests
    {
        [Test]
        public void CanCreateOrderModel()
        {
            var order = new Order
            {
                Id = "id1",
                BookId = "book1",
                Quantity = 2,
                UnitPrice = 10.5m,
                CreatedAtUtc = DateTime.UtcNow
            };
            Assert.That(order.Id, Is.EqualTo("id1"));
            Assert.That(order.BookId, Is.EqualTo("book1"));
            Assert.That(order.Quantity, Is.EqualTo(2));
            Assert.That(order.UnitPrice, Is.EqualTo(10.5m));
        }
    }
}
