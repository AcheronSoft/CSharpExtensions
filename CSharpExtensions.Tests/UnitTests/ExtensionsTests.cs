using System.Diagnostics;
using CSharpExtensions.Lib.Constants;
using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Tests.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class ExtensionsTests
{
    private readonly DefaultHttpContext _context = new();

    public ExtensionsTests()
    {
        _context.Request.Method = "GET";
        _context.Request.Path = "/api/test";
        _context.TraceIdentifier = "test-trace-id";
        _context.Request.Headers[CustomRequestHeaders.RequestId] = "test-request-id";
    }

    [Fact]
    public void CreateProblemDetails_FromError_ReturnsCorrectDetails()
    {
        // Arrange
        var error = new Error("Test error")
            .AsBadRequest("TestError", "Test Error Title")
            .WithTimestamp(new DateTime(2023, 1, 1));

        // Act
        var result = ExceptionExtensions.CreateProblemDetails(_context, error);

        // Assert
        Assert.Equal(error.Title, result.Title);
        Assert.Equal(error.Type, result.Type);
        Assert.Equal("GET /api/test", result.Instance);
        Assert.Equal(error.GetHttpStatusCode(), result.Status);
        Assert.Equal(error.Message, result.Detail);
        Assert.Equal(new DateTime(2023, 1, 1), result.Extensions["timestamp"]);
        Assert.Equal("test-trace-id", result.Extensions["traceId"]);
        Assert.Equal("test-request-id", result.Extensions["requestId"]);
        Assert.NotNull(result.Extensions["version"]);
    }

    [Fact]
    public void ToError_ConvertsProblemDetailsCorrectly()
    {
        // Arrange
        var problemDetails = new ProblemDetails
        {
            Type = "TestType",
            Title = "Test Title",
            Status = 400,
            Detail = "Test detail",
            Extensions = { ["timestamp"] = new DateTime(2023, 1, 1) }
        };

        // Act
        var error = problemDetails.ToError();

        // Assert
        Assert.Equal("TestType", error.Type);
        Assert.Equal("Test Title", error.Title);
        Assert.Equal(400, error.GetHttpStatusCode());
        Assert.Equal(new DateTime(2023, 1, 1), error.Timestamp);
    }

    [Fact]
    public void GetMessages_ReturnsAllUniqueMessages()
    {
        // Arrange
        var exception = new Exception("Level1",
            new Exception("Level2",
                new Exception("Level2"))); // Duplicate message

        // Act
        var result = exception.GetMessages(3);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("Level1", result);
        Assert.Contains("Level2", result);
    }

    [Fact]
    public void CreateProblemDetails_AddsActivityTraceIdWhenAvailable()
    {
        // Arrange
        using var activity = new Activity("TestActivity").Start();
        var expectedTraceId = activity.Id;

        // Act
        var result = ExceptionExtensions.CreateProblemDetails(_context, new Error("Test"));

        // Assert
        Assert.Equal(expectedTraceId, result.Extensions["traceId"]);
    }

    [Fact]
    public void ToError_HandlesDifferentStatusCodesCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            (400, "UnauthorizedError"),
            (401, "UnauthorizedError"),
            (403, "ForbiddenError"),
            (404, "NotFoundError"),
            (499, "TestType"),
            (500, "TestType")
        };

        foreach (var (statusCode, expectedType) in testCases)
        {
            var problemDetails = new ProblemDetails
            {
                Type = expectedType,
                Title = "TestTitle",
                Status = statusCode,
                Detail = "TestMessage"
            };

            // Act
            var error = problemDetails.ToError();
            var expectedStatusCode = statusCode is 400 or > 404 and < 500 ? 400 : statusCode;
            // Assert
            Assert.Equal(expectedType, error.Type);
            Assert.Equal(expectedStatusCode, error.GetHttpStatusCode());
        }
    }

    [Fact]
    public void LogError_WithMetadata_AddsMetadataToScope()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var capturedScopes = new List<Dictionary<string, string>>();
        var error = new Error("Test message")
            .WithMetadata("key1", "value1")
            .WithMetadata("key2", 123);

        // Настройка захвата параметров для BeginScope
        loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, string>>()))
            .Callback<Dictionary<string, string>>(scope => capturedScopes.Add(scope))
            .Returns(Mock.Of<IDisposable>);

        // Act
        loggerMock.Object.LogError(error);

        // Assert
        Assert.Single(capturedScopes);
        var metadata = capturedScopes[0]["metadata"];
        Assert.Contains("key1='\"value1\"'", metadata);
        Assert.Contains("key2='123'", metadata);
    }

     [Fact]
    public void CreateValidationProblemDetails_ShouldPopulateAllProperties()
    {
        // Arrange
        var traceId = "00-f383d568e2345d8648248392f97ed5e1-eec69f31e04bf509-00";
        var requestId = Guid.NewGuid().ToString();

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(httpContext => httpContext.Request.Method).Returns("POST");
        mockHttpContext.Setup(httpContext => httpContext.Request.Path).Returns("/api/test");
        mockHttpContext.Setup(httpContext => httpContext.Request.Headers[CustomRequestHeaders.RequestId]).Returns(requestId);
        mockHttpContext.Setup(httpContext => httpContext.Request.ContentType).Returns("application/problem+json; v=1");
        mockHttpContext.SetupGet(h => h.TraceIdentifier).Returns(traceId);

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Field is required");

        var actionContext = new ActionContext(
            mockHttpContext.Object,
            new RouteData(),
            new Mock<ActionDescriptor>().Object,
            modelState
        );

        // Act
        var result = ExceptionExtensions.CreateValidationProblemDetails(actionContext);

        // Assert
        Assert.Equal("Произошла ошибка валидации", result.Title);
        Assert.Equal("ValidationError", result.Type);
        Assert.Equal("POST /api/test", result.Instance);
        Assert.Equal(400, result.Status);
        Assert.Equal("Не все параметры запроса удолетворяют условиям.", result.Detail);

        // Проверка расширений
        Assert.IsType<DateTime>(result.Extensions["timestamp"]);
        Assert.Equal(traceId, result.Extensions["traceId"]);
        Assert.Equal(requestId, result.Extensions["requestId"]);
        Assert.Equal("1", result.Extensions["version"]); // Зависит от реализации GetVersionOfApi

        // Проверка валидационных ошибок
        Assert.Contains("Name", result.Errors.Keys);
        Assert.Contains("Field is required", result.Errors["Name"]);
    }
}


