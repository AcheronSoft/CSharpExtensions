using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Tests.Base;
using FluentAssertions;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class HttpTests
{
    public HttpTests(SharedFixture sharedFixture)
    {
    }

    [Theory]
    [InlineData("v=123; Какой-то текст")]
    [InlineData("application/json; v=123")]
    [InlineData("application/problem+json; v=123")]
    [InlineData("application/problem+json; v=123; НеведомаяХрень")]
    public void GetApiVersionFromContentTypeTest_Positive(string contentType)
    {
        var version = HttpContextExtensions.GetVersionOfApiFromContentType(contentType);

        version.Should().Be("123");
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json;v=")]
    public void GetApiVersionFromContentTypeTest_Negative(string contentType)
    {
        var version = HttpContextExtensions.GetVersionOfApiFromContentType(contentType);

        version.Should().BeEmpty();
    }
}
