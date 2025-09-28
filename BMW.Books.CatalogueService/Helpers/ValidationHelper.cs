using System.ComponentModel.DataAnnotations;

namespace BMW.Books.CatalogueService.Helpers;

public static class ValidationHelper
{
    public static IResult Validate<T>(T dto)
    {
        var ctx = new ValidationContext(dto!);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(dto!, ctx, results, true))
        {
            return Results.ValidationProblem(results.ToDictionary(r => r.MemberNames.FirstOrDefault() ?? "field", r => new[] { r.ErrorMessage ?? "Invalid" }));
        }
        return Results.Ok();
    }
}
