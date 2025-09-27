using BMW.Books.CatalogueService.Models;
using BMW.Books.CatalogueService.Services;

namespace BMW.Books.CatalogueService.Endpoints
{
    public static class BookEndpoints
    {
        public static void MapBookEndpoints(this WebApplication app)
        {
            app.MapGet("/books/{id}", (string id, IBookService bookService) =>
            {
                var book = bookService.GetBookById(id);
                return book is not null ? Results.Ok(book) : Results.NotFound();
            }).RequireRateLimiting("general");

            app.MapGet("/books", (IBookService bookService) => bookService.GetAllBooks()).RequireRateLimiting("general");

            app.MapPost("/books", async (BookRequest book, IBookService bookService) =>
            {
                var added = await bookService.AddBookAsync(book);
                return Results.Created($"/books/{added.Id}", added);
            }).RequireRateLimiting("general");
        }
    }
}
