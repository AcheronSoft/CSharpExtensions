using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Lib.Railway.Configurations;
using CSharpExtensions.Lib.Railway.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpExtensions.Lib.Extensions;

public static class WebApplicationExtensions
{
    public static IServiceCollection AddApiRailway(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IActionResultProfile, ActionResultProfile>();

        return services;
    }

    public static WebApplication UseApiRailway(this WebApplication app)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

        RailwayConfiguration.Setup(x => x.CurrentProfile = new ActionResultProfile(httpContextAccessor));

        return app;
    }
}
