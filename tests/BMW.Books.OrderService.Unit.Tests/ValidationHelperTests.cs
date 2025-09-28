using BMW.Books.OrderService.Helpers;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace BMW.Books.OrderService.Unit.Tests
{
    public class ValidationHelperTests
    {
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

        [Test]
        public void Validate_ReturnsOk_ForValidDto()
        {
            var dto = new TestDtoValid();
            var result = ValidationHelper.Validate(dto);
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.GetType().Name, Is.EqualTo("Ok"));
        }

        [Test]
        public void Validate_ReturnsValidationProblem_ForInvalidDto()
        {
            var dto = new TestDtoInvalid();
            var result = ValidationHelper.Validate(dto);
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.GetType().Name, Is.EqualTo("ProblemHttpResult"));
        }
    }
}
