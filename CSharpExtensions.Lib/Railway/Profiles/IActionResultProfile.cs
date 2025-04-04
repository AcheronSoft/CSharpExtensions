using CSharpExtensions.Lib.Railway.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExtensions.Lib.Railway.Profiles;

public interface IActionResultProfile
{
    ActionResult TransformToSuccessActionResult(ActionResultProfileSuccessContext context);

    ActionResult<T> TransformToSuccessActionResult<T>(ActionResultProfileSuccessContext<T> context);

    ActionResult TransformToFailureActionResult(ActionResultProfileFailureContext context);
}
