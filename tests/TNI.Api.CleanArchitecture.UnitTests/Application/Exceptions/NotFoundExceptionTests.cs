using FluentAssertions;
using TNI.Api.CleanArchitecture.Application.Exceptions;
using Xunit;

namespace TNI.Api.CleanArchitecture.UnitTests.Application.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_Message_ShouldMatchExpectedFormat()
    {
        // Arrange & Act
        var exception = new NotFoundException("User", Guid.Parse("00000000-0000-0000-0000-000000000001"));

        // Assert
        exception.Message.Should().Be("Entity 'User' (00000000-0000-0000-0000-000000000001) was not found.");
    }

    [Fact]
    public void NotFoundException_ShouldBeException()
    {
        var exception = new NotFoundException("Product", 42);
        exception.Should().BeAssignableTo<Exception>();
    }
}
