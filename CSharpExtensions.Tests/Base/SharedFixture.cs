using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSharpExtensions.Tests.Base;

public class SharedFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "local"
        });

        builder.Services.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddConsole();
            });
        
        builder.WebHost.UseTestServer();

        var host = builder.Build();

        await host.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
