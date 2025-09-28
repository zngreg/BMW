using BMW.Books.OrderService.Models;
using BMW.Books.OrderService.Services;

namespace BMW.Books.OrderService.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app, Func<object, IResult> validate)
        {
            app.MapGet("/orders/{id}", async (string id, IOrderService orderService) =>
            {
                return await orderService.GetOrderByIdAsync(id);
            });

            app.MapGet("/orders", async (IOrderService orderService) =>
            {
                return await orderService.GetAllOrdersAsnyc();
            });

            app.MapPost("/orders", async (OrderRequest req, IOrderService orderService) =>
            {
                var v = validate(req); if (v is IResult res && res.GetType() != Results.Ok().GetType()) return res;

                var order = await orderService.CreateOrderAsync(req);
                return order.IsSuccess is true ? Results.Created($"/orders/{order.Data?.Id}", order) : Results.BadRequest(order);
            });
        }
    }
}