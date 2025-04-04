using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Lib.Railway.Contexts;
using CSharpExtensions.Lib.Railway.Profiles;
using CSharpExtensions.Tests.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class ActionResultProfileTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly DefaultHttpContext _httpContext = new();

    [Fact]
    public void TransformToSuccessActionResult_ReturnsCorrectResult()
    {
        // Arrange
        var profile = new ActionResultProfile(_httpContextAccessorMock.Object);
        var context = new ActionResultProfileSuccessContext<string>(Result.Success("test"));

        // Act
        var actionResult = profile.TransformToSuccessActionResult(context);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal("test", objectResult.Value);
    }

    [Fact]
    public void TransformToFailureActionResult_ReturnsCorrectProblemDetails()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);
        var profile = new ActionResultProfile(_httpContextAccessorMock.Object);
        var error = new Error("Test error").AsBadRequest("Type", "Title");

        // Act
        var result = profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(error));

        // Assert
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Title", problemDetails.Title);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public void TransformToFailureActionResult_ThrowsWhenHttpContextNull()
    {
        // Arrange
        var profile = new ActionResultProfile(_httpContextAccessorMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(new Error("Error"))));
    }
}
