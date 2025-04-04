using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Tests.TestObjects;
using CSharpExtensions.Tests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class ErrorTests
{
    [Fact]
    public void ErrorCheck_Success()
    {
        var error = Stabs.MyTestError();

        error.Message.Should().Be("Test Message");
        error.Type.Should().Be("TestMessageError");
        error.Title.Should().Be("Уж случилось так случилось.");
        error.Details.Count.Should().Be(2);
        error.Details.Should().Contain("Ох бабаньки, штошь делаетси.");
        error.Details.Should().Contain("Произошла какая-то неведомая дичь!");
    }

     [Fact]
    public void Constructor_WithValidMessage_InitializesCorrectly()
    {
        // Arrange & Act
        var error = new Error("Test error message");

        // Assert
        Assert.Equal("Test error message", error.Message);
        Assert.Equal("InternalServerError", error.Type);
        Assert.Equal("Произошла серверная ошибка.", error.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, error.GetHttpStatusCode());
        Assert.NotEmpty(error.Timestamp.ToString());
        Assert.Empty(error.Details);
        Assert.Empty(error.StackTraces);
        Assert.Empty(error.Metadata);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidMessage_ThrowsException(string message)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Error(message));
        Assert.Contains(nameof(Error.Message), ex.Message);
    }

    [Fact]
    public void AsBadRequest_ValidParameters_SetsPropertiesCorrectly()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.AsBadRequest("ValidationError", "Invalid request");

        // Assert
        Assert.Equal("ValidationError", error.Type);
        Assert.Equal("Invalid request", error.Title);
        Assert.Equal(StatusCodes.Status400BadRequest, error.GetHttpStatusCode());
    }

    [Theory]
    [InlineData(null, "title")]
    [InlineData("", "title")]
    [InlineData("type", null)]
    [InlineData("type", "")]
    public void AsBadRequest_InvalidParameters_ThrowsException(string type, string title)
    {
        // Arrange
        var error = new Error("Error");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => error.AsBadRequest(type, title));
    }

    [Fact]
    public void AsUnauthorized_SetsCorrectProperties()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.AsUnauthorized();

        // Assert
        Assert.Equal("UnauthorizedError", error.Type);
        Assert.Equal(StatusCodes.Status401Unauthorized, error.GetHttpStatusCode());
    }

    [Fact]
    public void AsForbidden_SetsCorrectProperties()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.AsForbidden();

        // Assert
        Assert.Equal("ForbiddenError", error.Type);
        Assert.Equal(StatusCodes.Status403Forbidden, error.GetHttpStatusCode());
    }

    [Fact]
    public void AsNotFound_SetsCorrectProperties()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.AsNotFound();

        // Assert
        Assert.Equal("NotFoundError", error.Type);
        Assert.Equal(StatusCodes.Status404NotFound, error.GetHttpStatusCode());
    }

    [Fact]
    public void AsInternalServer_ValidParameters_SetsProperties()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.AsInternalServer("DatabaseError", "Database failure");

        // Assert
        Assert.Equal("DatabaseError", error.Type);
        Assert.Equal("Database failure", error.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, error.GetHttpStatusCode());
    }

    [Fact]
    public void AsInternalServer_InvalidParameters_ThrowsException()
    {
        // Arrange
        var error = new Error("Error");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => error.AsInternalServer(null, null));
    }

    [Fact]
    public void WithDetails_AddsUniqueDetails()
    {
        // Arrange
        var error = new Error("Main error");

        // Act
        error.WithDetails("Detail 1")
            .WithDetails("Detail 1") // Duplicate
            .WithDetails("Detail 2")
            .WithDetails("Main error"); // Same as main message

        // Assert
        Assert.Equal(2, error.Details.Count);
        Assert.Contains("Detail 1", error.Details);
        Assert.Contains("Detail 2", error.Details);
    }

    [Fact]
    public void CausedBy_AddsExceptionDetailsAndStackTraces()
    {
        // Arrange
        var error = new Error("Main error");
        var exception = new Exception("Inner exception", new Exception("Root exception"));

        // Act
        error.CausedBy(exception);

        // Assert
        Assert.Equal(2, error.Details.Count);
        Assert.Contains("Inner exception", error.Details);
        Assert.Contains("Root exception", error.Details);
    }

    [Fact]
    public void WithMetadata_AddsAndUpdatesMetadata()
    {
        // Arrange
        var error = new Error("Error");

        // Act
        error.WithMetadata("key1", "value1")
            .WithMetadata("key2", 42)
            .WithMetadata("key1", "updated");

        // Assert
        Assert.Equal("updated", error.Metadata["key1"]);
        Assert.Equal(42, error.Metadata["key2"]);
    }

    [Fact]
    public void WithMetadata_DictionaryAddsAllItems()
    {
        // Arrange
        var error = new Error("Error");
        var metadata = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = 42
        };

        // Act
        error.WithMetadata(metadata);

        // Assert
        Assert.Equal(2, error.Metadata.Count);
        Assert.Equal("value1", error.Metadata["key1"]);
        Assert.Equal(42, error.Metadata["key2"]);
    }

    [Fact]
    public void HasMetadataKey_ReturnsCorrectValue()
    {
        // Arrange
        var error = new Error("Error").WithMetadata("testKey", "value");

        // Act & Assert
        Assert.True(error.HasMetadataKey("testKey"));
        Assert.False(error.HasMetadataKey("nonExistingKey"));
    }

    [Fact]
    public void HasMetadata_WithPredicate_WorksCorrectly()
    {
        // Arrange
        var error = new Error("Error").WithMetadata("age", 30);

        // Act & Assert
        Assert.True(error.HasMetadata("age", v => (int)v == 30));
        Assert.False(error.HasMetadata("age", v => (int)v > 40));
    }

    [Fact]
    public void WithTimestamp_SetsNewTimestamp()
    {
        // Arrange
        var error = new Error("Error");
        var newTime = new DateTime(2023, 1, 1);

        // Act
        error.WithTimestamp(newTime);

        // Assert
        Assert.Equal(newTime, error.Timestamp);
    }

    [Fact]
    public void GetHttpStatusCode_ReturnsCorrectValue()
    {
        // Arrange
        var error = new Error("Error").AsNotFound();

        // Act & Assert
        Assert.Equal(StatusCodes.Status404NotFound, error.GetHttpStatusCode());
    }


    [Fact]
    public void ErrorNone_ReturnsDefaultError()
    {
        // Arrange
        var error = Error.None;

        // Act & Assert
        Assert.Null(error.Message);
        Assert.Null(error.Type);
        Assert.Null(error.Title);
        Assert.Equal(DateTime.MinValue, error.Timestamp);
        Assert.Equal(0, error.GetHttpStatusCode());
    }

    [Fact]
    public void ErrorNone_ReturnEqual()
    {
        // Arrange
        var error1 = Error.None;
        var error2 = Error.None;

        // Act & Assert
        Assert.Equal(error1, error2);
        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }

    [Fact]
    public void Error_ReturnEqual()
    {
        // Arrange
        var ex = new ArgumentException("Произошла какая-то неведомая дичь!");

        var error1 = Stabs.MyTestError();
        var error2 = new Error("Test Message")
            .AsInternalServer("TestMessageError", "Уж случилось так случилось.")
            .WithDetails("Ох бабаньки, штошь делаетси.")
            .WithMetadata("TestKeyMetadata", "TestValueMetadata")
            .CausedBy(ex);

        // Act & Assert
        Assert.Equal(error1, error2);
        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }
}
