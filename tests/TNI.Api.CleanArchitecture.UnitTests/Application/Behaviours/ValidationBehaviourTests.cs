using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using TNI.Api.CleanArchitecture.Application.Common.Behaviours;
using Xunit;
using ValidationException = TNI.Api.CleanArchitecture.Application.Exceptions.ValidationException;
using FluentValidation;

namespace TNI.Api.CleanArchitecture.UnitTests.Application.Behaviours;

public record TestRequest(string Value) : IRequest<string>;

public class ValidationBehaviourTests
{

    [Fact]
    public async Task Handle_WithNoValidators_ShouldCallNext()
    {
        // Arrange
        var behaviour = new ValidationBehaviour<TestRequest, string>(Enumerable.Empty<IValidator<TestRequest>>());
        var nextCalled = false;
        var request = new TestRequest("valid");

        // Act
        var result = await behaviour.Handle(request, ct =>
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ShouldThrowValidationException()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Value", "Value cannot be empty.")
            }));

        var behaviour = new ValidationBehaviour<TestRequest, string>(new[] { mockValidator.Object });
        var request = new TestRequest("");

        // Act
        var act = async () => await behaviour.Handle(request, ct => Task.FromResult("ok"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Value"));
    }

    [Fact]
    public async Task Handle_WithPassingValidator_ShouldCallNext()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behaviour = new ValidationBehaviour<TestRequest, string>(new[] { mockValidator.Object });
        var nextCalled = false;
        var request = new TestRequest("valid");

        // Act
        await behaviour.Handle(request, ct =>
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }
}
