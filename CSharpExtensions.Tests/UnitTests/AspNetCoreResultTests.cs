using CSharpExtensions.Lib.Railway.Configurations;
using CSharpExtensions.Lib.Railway.Profiles;
using Microsoft.AspNetCore.Http;
using CSharpExtensions.Tests.Base;
using Moq;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class AspNetCoreResultTests
{
    [Fact]
    public void Setup_ConfiguresDefaultProfile()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
        var profile = new ActionResultProfile(httpContextAccessor);

        // Act
        RailwayConfiguration.Setup(settings => settings.CurrentProfile = new ActionResultProfile(httpContextAccessor));

        // Assert
        Assert.Equal(profile, RailwayConfiguration.GetCurrentProfile());
    }
}
