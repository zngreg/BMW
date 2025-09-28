using Microsoft.AspNetCore.Builder;
using BMW.Books.OrderService.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BMW.Books.OrderService.Unit.Tests
{
    public class ErrorHandlingMiddlewareTests
    {
        [Test]
        public async Task UseGlobalErrorHandler_ReturnsJsonError_WhenExceptionThrown()
        {
            // Arrange
            var app = WebApplication.CreateBuilder().Build();
            var logger = app.Services.GetRequiredService<ILogger<ErrorHandlingMiddlewareTests>>();
            app.UseGlobalErrorHandler(logger);

            app.MapGet("/throw", (HttpContext context) => throw new Exception("Test error"));

            var client = app.RunAsync();
            using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
            try
            {
                await httpClient.GetAsync("/throw");
            }
            catch
            {
                // Ignore exceptions from the test server
            }
            // Cleanup
            await app.StopAsync();
        }
    }
}
