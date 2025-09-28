namespace BMW.Books.CatalogueService.Endpoints
{
    public static class HealthEndpoints
    {
        public static void MapHealthEndpoints(this WebApplication app)
        {
            app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
        }
    }
}
