using BMW.Books.OrderService.Models;
using BMW.Books.OrderService.Services;

namespace BMW.Books.OrderService.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            app.MapGet("/orders/{id}", (string id, IOrderService orderService) =>
            {
                var order = orderService.GetOrderById(id);
                return order is not null ? Results.Ok(order) : Results.NotFound();
            });

            app.MapPost("/orders", async (OrderRequest req, IOrderService orderService) =>
            {
                var order = await orderService.CreateOrderAsync(req);
                return order is not null ? Results.Created($"/orders/{order.Id}", order) : Results.BadRequest(new { error = "Could not create order. Invalid book ID?" });
            });
        }
    }
}