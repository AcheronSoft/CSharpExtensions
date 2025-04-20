using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Lib.Railway.Configurations;
using CSharpExtensions.Lib.Railway.Profiles;
using Microsoft.AspNetCore.Http;
using CSharpExtensions.Tests.Base;
using Moq;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class ResultExtensionsTests
{
    private static readonly IHttpContextAccessor HttpContextAccessor = new Mock<IHttpContextAccessor>().Object;
    private readonly IActionResultProfile _profile = new ActionResultProfile(HttpContextAccessor);

    [Fact]
    public async Task ToActionResult_WithTaskResult_ReturnsCorrectActionResult()
    {
        // Arrange
        var taskResult = Task.FromResult(Result.Success());

        // Act
        var result = await taskResult.ToActionResult(_profile);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ToActionResult_WithoutProfile_UsesDefaultProfile()
    {
        // Arrange
        RailwayConfiguration.Setup(settings => settings.CurrentProfile = _profile);
        var result = Result.Success();

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        Assert.NotNull(actionResult);
    }
}
