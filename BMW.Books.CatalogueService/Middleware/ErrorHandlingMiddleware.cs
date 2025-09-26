using Microsoft.AspNetCore.Diagnostics;

namespace BMW.Books.CatalogueService.Middleware
{
    public static class ErrorHandlingMiddleware
    {
        public static void UseGlobalErrorHandler(this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var result = System.Text.Json.JsonSerializer.Serialize(new { error = error.Error.Message });
                        await context.Response.WriteAsync(result);
                    }
                });
            });
        }
    }
}
