using CSharpExtensions.Lib.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CSharpExtensions.Lib.ExceptionHandlers;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = ExceptionExtensions.CreateProblemDetails(httpContext, exception);

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        
        var json = problemDetails.ToJson();

        logger.LogError(exception, json);
        
        await httpContext.Response.WriteAsync(json, cancellationToken);
        
        await httpContext.Response.CompleteAsync();
        
        return true;
    }
}