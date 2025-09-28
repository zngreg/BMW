using BMW.Books.CatalogueService.Models;
using BMW.Books.CatalogueService.Services;

namespace BMW.Books.CatalogueService.Endpoints
{
    public static class BookEndpoints
    {
        public static void MapBookEndpoints(this WebApplication app, Func<object, IResult> validate)
        {
            app.MapGet("/books/{isbn}", async (string isbn, IBookService bookService) =>
            {
                return await bookService.GetBookByISBNAsync(isbn);
            });

            app.MapGet("/books", async (IBookService bookService) => await bookService.GetAllBooksAsync());

            app.MapPost("/books", async (BookRequest book, IBookService bookService) =>
            {
                var v = validate(book); if (v is IResult res && res.GetType() != Results.Ok().GetType()) return res;

                var added = await bookService.AddBookAsync(book);
                return added.IsSuccess ? Results.Created($"/books/{added.Data?.ISBN}", added) : Results.BadRequest(added);
            });
        }
    }
}
