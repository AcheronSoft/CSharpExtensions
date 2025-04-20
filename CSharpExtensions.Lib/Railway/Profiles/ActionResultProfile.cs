using CSharpExtensions.Lib.CustomObjectResults;
using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Lib.Railway.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExtensions.Lib.Railway.Profiles;

public record ActionResultProfile : IActionResultProfile
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActionResultProfile(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ActionResult TransformToSuccessActionResult(ActionResultProfileSuccessContext context)
    {
        return new ObjectResult(null) { StatusCode = StatusCodes.Status200OK };
    }

    public ActionResult<T> TransformToSuccessActionResult<T>(ActionResultProfileSuccessContext<T> context)
    {
        return new ObjectResult(context.Result.ValueOrDefault) { StatusCode = StatusCodes.Status200OK };
    }

    public ActionResult TransformToFailureActionResult(ActionResultProfileFailureContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            throw new ArgumentException("Не удалось получить HttpContext из IHttpContextAccessor для возрата данных в ActionResult. Убедитесь, что HttpContextAccessor зарегистрирован.");

        var problemDetails = ExceptionExtensions.CreateProblemDetails(httpContext, context.Error);

        return problemDetails.Status switch
        {
            400 => new BadRequestObjectResult(problemDetails),
            401 => new UnauthorizedObjectResult(problemDetails),
            403 => new ForbiddenObjectResult(problemDetails),
            404 => new NotFoundObjectResult(problemDetails),
            _   => new InternalServerErrorObjectResult(problemDetails),
        };
    }
}
