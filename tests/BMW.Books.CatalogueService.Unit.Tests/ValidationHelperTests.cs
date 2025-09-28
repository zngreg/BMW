using BMW.Books.CatalogueService.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace BMW.Books.CatalogueService.Unit.Tests
{
    [TestFixture]
    public class ValidationHelperTests
    {
        [Test]
        public void Validate_With_ValidDto_Should_Return_Ok()
        {
            var dto = new TestDtoValid();
            var result = ValidationHelper.Validate(dto);
            Assert.That(result, Is.InstanceOf<Ok>());
        }

        [Test]
        public void Validate_With_InvalidDto_Should_Return_ProblemHttpResult()
        {
            var dto = new TestDtoInvalid();
            var result = ValidationHelper.Validate(dto);
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
        }

        private class TestDtoValid
        {
            [Required]
            public string Name { get; set; } = "ValidName";
        }

        private class TestDtoInvalid
        {
            [Required]
            public string Name { get; set; }
        }
    }
}
