using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Tests.Base;
using CSharpExtensions.Tests.TestObjects;
using FluentAssertions;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class ResultTests
{
    [Fact]
    public void ResultOperatorTest()
    {
        var result = Stabs.TestResult();

        result.Error.Message.Should().Be("Test Message");
        result.Error.Type.Should().Be("TestMessageError");
        result.Error.Title.Should().Be("Уж случилось так случилось.");
        result.Error.Details.Count.Should().Be(2);
        result.Error.Details.Should().Contain("Ох бабаньки, штошь делаетси.");
        result.Error.Details.Should().Contain("Произошла какая-то неведомая дичь!");
    }

    [Fact]
    public void ResultTOperatorTest()
    {
        var result = Stabs.TestResultT();

        result.Error.Message.Should().Be("Test Message");
        result.Error.Type.Should().Be("TestMessageError");
        result.Error.Title.Should().Be("Уж случилось так случилось.");
        result.Error.Details.Count.Should().Be(2);
        result.Error.Details.Should().Contain("Ох бабаньки, штошь делаетси.");
        result.Error.Details.Should().Contain("Произошла какая-то неведомая дичь!");
    }

    [Fact]
    public void Success_ReturnsCorrectResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_WithError_ReturnsCorrectResult()
    {
        // Arrange
        var error = new Error("Test error");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailure()
    {
        // Arrange
        Error error = new("Test error");

        // Act
        Result result = error;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_ThrowsWhenUnsuccessful()
    {
        // Arrange
        var result = Result.Failure<int>(new Error("Error"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ValueOrDefault_ReturnsDefaultWhenUnsuccessful()
    {
        // Arrange
        var result = Result.Failure<int>(new Error("Error"));

        // Act
        var value = result.ValueOrDefault;

        // Assert
        Assert.Equal(default, value);
    }

    [Fact]
    public void Create_WithNullValue_ReturnsFailure()
    {
        // Act
        var result = Result.Create<string>(null);

        // Assert
        Assert.True(result.IsFailure);
    }
}
