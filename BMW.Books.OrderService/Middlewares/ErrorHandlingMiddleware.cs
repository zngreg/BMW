using BMW.Books.OrderService.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace BMW.Books.OrderService.Middlewares
{
    public static class ErrorHandlingMiddleware
    {
        public static void UseGlobalErrorHandler(this WebApplication app, ILogger logger)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        logger.LogError(error.Error, "An unhandled exception occurred.");

                        var result = System.Text.Json.JsonSerializer.Serialize(new ResponseModel<string> { IsSuccess = false, Reason = error.Error.Message });
                        await context.Response.WriteAsync(result);
                    }
                });
            });
        }
    }
}
