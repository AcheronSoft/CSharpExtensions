using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExtensions.Lib.CustomObjectResults;

public class ForbiddenObjectResult : ObjectResult
{
    public ForbiddenObjectResult(object? value) : base(value)
    {
        StatusCode = StatusCodes.Status403Forbidden;
    }
}
