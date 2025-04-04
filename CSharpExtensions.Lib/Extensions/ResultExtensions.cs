using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Lib.Railway.Configurations;
using CSharpExtensions.Lib.Railway.Profiles;
using CSharpExtensions.Lib.Railway.Transformers;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExtensions.Lib.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result, IActionResultProfile profile)
    {
        return new ActionResultTransformer().Transform(result, profile);
    }

    public static async Task<ActionResult> ToActionResult(this Task<Result> resultTask, IActionResultProfile profile)
    {
        var result = await resultTask;
        return new ActionResultTransformer().Transform(result, profile);
    }

    public static ActionResult ToActionResult(this Result result)
    {
        return ToActionResult(result, RailwayConfiguration.ActionResultProfileSettings.CurrentProfile);
    }

    public static async Task<ActionResult> ToActionResult(this Task<Result> resultTask)
    {
        var result = await resultTask;
        return ToActionResult(result, RailwayConfiguration.ActionResultProfileSettings.CurrentProfile);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, IActionResultProfile profile)
    {
        return new ActionResultTransformer().Transform(result, profile);
    }

    public static async Task<ActionResult<T>> ToActionResult<T>(this Task<Result<T>> resultTask, IActionResultProfile profile)
    {
        var result = await resultTask;
        return new ActionResultTransformer().Transform(result, profile);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        return ToActionResult(result, RailwayConfiguration.ActionResultProfileSettings.CurrentProfile);
    }

    public static async Task<ActionResult<T>> ToActionResult<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return ToActionResult(result, RailwayConfiguration.ActionResultProfileSettings.CurrentProfile);
    }
}
