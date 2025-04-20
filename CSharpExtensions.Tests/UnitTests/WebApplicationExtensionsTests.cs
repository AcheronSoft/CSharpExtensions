using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Lib.Railway.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CSharpExtensions.Tests.Base;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class WebApplicationExtensionsTests
{
    [Fact]
    public void AddApiRailway_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApiRailway();

        // Assert
        Assert.Contains(services, s => s.ServiceType == typeof(IHttpContextAccessor));
        Assert.Contains(services, s => s.ServiceType == typeof(IActionResultProfile));
    }
}
