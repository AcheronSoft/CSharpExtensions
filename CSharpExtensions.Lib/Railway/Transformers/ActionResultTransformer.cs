using CSharpExtensions.Lib.Railway.Contexts;
using CSharpExtensions.Lib.Railway.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExtensions.Lib.Railway.Transformers;

public class ActionResultTransformer
{
    public ActionResult Transform(Result result, IActionResultProfile profile)
    {
        return result.IsFailure
            ? profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(result.Error))
            : profile.TransformToSuccessActionResult(new ActionResultProfileSuccessContext(result));
    }

    public ActionResult<T> Transform<T>(Result<T> result, IActionResultProfile profile)
    {
        return result.IsFailure
            ? profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(result.Error))
            : profile.TransformToSuccessActionResult<T>(new ActionResultProfileSuccessContext<T>(result));
    }
}
