using System.Diagnostics;
using CSharpExtensions.Lib.Constants;
using CSharpExtensions.Lib.Exceptions;
using CSharpExtensions.Lib.Railway;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CSharpExtensions.Lib.Extensions;

public static class ExceptionExtensions
{
    public static ProblemDetails CreateProblemDetails(in HttpContext httpContext, in Error error)
    {
        var problemDetails = new ProblemDetails();

        problemDetails.Title = error.Title;
        problemDetails.Type = error.Type;
        problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
        problemDetails.Status = error.GetHttpStatusCode();
        problemDetails.Detail = error.Message;

        problemDetails.Extensions.Add("timestamp", error.Timestamp);

        if (!problemDetails.Extensions.ContainsKey("traceId"))
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions.Add("traceId", traceId);
        }

        problemDetails.Extensions.Add("requestId", httpContext.Request.Headers[CustomRequestHeaders.RequestId].ToString());

        var version = HttpContextExtensions.GetVersionOfApiFromContentType(httpContext.Request.ContentType);

        problemDetails.Extensions.Add("version", version);

        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(ActionContext actionContext)
    {
        return CreateValidationProblemDetails(actionContext.HttpContext, actionContext.ModelState);
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelState)
    {
        var problemDetails = new ValidationProblemDetails(modelState);

        problemDetails.Title = "Произошла ошибка валидации";
        problemDetails.Type = "ValidationError";
        problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
        problemDetails.Status = StatusCodes.Status400BadRequest;
        problemDetails.Detail = "Не все параметры запроса удолетворяют условиям.";

        problemDetails.Extensions.Add("timestamp", DateTime.Now);

        if (!problemDetails.Extensions.ContainsKey("traceId"))
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions.Add("traceId", traceId);
        }

        problemDetails.Extensions.Add("requestId", httpContext.Request.Headers[CustomRequestHeaders.RequestId].ToString());

        var version = HttpContextExtensions.GetVersionOfApiFromContentType(httpContext.Request.ContentType);

        problemDetails.Extensions.Add("version", version);

        return problemDetails;
    }

    public static ProblemDetails CreateProblemDetails(in HttpContext httpContext, in Exception exception)
    {
        var error = new Error(exception.Message);

        switch (exception)
        {
            case BadRequestException badRequestException:
                error.AsBadRequest(badRequestException.ErrorType, badRequestException.ErrorTitle);
                break;
            case UnauthorizedException:
                error.AsUnauthorized();
                break;
            case ForbiddenException:
                error.AsForbidden();
                break;
            case NotFoundException:
                error.AsNotFound();
                break;
            case InternalServerException internalServerException when
                !StringExtensions.IsNullOrEmptyOrWhiteSpace(internalServerException.ErrorType) &&
                !StringExtensions.IsNullOrEmptyOrWhiteSpace(internalServerException.ErrorTitle):
                error.AsInternalServer(internalServerException.ErrorType, internalServerException.ErrorTitle);
                break;
        }

        return CreateProblemDetails(httpContext, error);
    }

    /// <summary>
    /// Конвертация <see cref="ProblemDetails"/> в <see cref="Error"/>
    /// </summary>
    public static Error ToError(this ProblemDetails problemDetails)
    {
        var error = new Error(problemDetails.Detail);

        var type = problemDetails.Type;
        var title = problemDetails.Title;

        switch (problemDetails.Status)
        {
            case 401:
                error.AsUnauthorized();
                break;
            case 403:
                error.AsForbidden();
                break;
            case 404:
                error.AsNotFound();
                break;
            case >= 400 and < 500:
                error.AsBadRequest(type, title);
                break;
            default:
                error.AsInternalServer(type, title);
                break;
        }

        if (problemDetails.Extensions.TryGetValue("timestamp", out var value))
        {
            if (value is DateTime timestamp)
            {
                error.WithTimestamp(timestamp);
            }
        }

        return error;
    }

    /// <summary>
    /// Получить все сообщения со всех вложенных исключений
    /// </summary>
    /// <param name="exception">Исключение</param>
    /// <param name="level">Уровень вложенности исключения</param>
    /// <returns>Тесктовый массив сообщений</returns>
    public static List<string> GetMessages(this Exception exception, int level = int.MaxValue)
    {
        var list = new List<string>();
        var counter = 1;
        while (exception != null && counter <= level)
        {
            list.Add(exception.Message);

            if (exception.InnerException == null)
                break;

            exception = exception.InnerException;
            counter++;
        }

        return list.Distinct().ToList();
    }

    /// <summary>
    /// Получить только целевые строки из StackTrace с указанием места ошибки
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static List<string> GetStackTrace(this Exception exception, int level = int.MaxValue)
    {
        var list = new List<string>();
        var counter = 1;
        while (exception != null && counter <= level)
        {
            var lines = GetLinesFromStackTrace(exception);

            if (lines.Count == 0)
               break;

            list.AddRange(lines);

            if (exception.InnerException == null)
                break;

            exception = exception.InnerException;
            counter++;
        }

        return list.Distinct().ToList();
    }

    private static List<string> GetLinesFromStackTrace(Exception exception)
    {
        var results = new List<string>();

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(exception?.StackTrace))
            return results;

        var lines = exception!.StackTrace!.Split(Environment.NewLine)
            .Where(x => x.Contains(":line ")).ToList();

        if (lines.Count == 0)
            return results;

        results.AddRange(lines.Select(GetTextFromLine));

        return results;
    }

    private static string GetTextFromLine(string line)
    {
        var length = line.Length;
        var index = line.IndexOf(") in ", StringComparison.Ordinal);
        var leftPartLength = line.Left(index).Length;
        return line.Right(length - leftPartLength - 5);
    }

    /// <summary>
    /// Корректное логирование объекта <see cref="Error"/>
    /// </summary>
    /// <param name="logger">Логгер</param>
    /// <param name="error">Объект ошибки</param>
    public static void LogError(this ILogger logger, Error error)
    {
        using (logger.BeginScope(new Dictionary<string, string>()
        {
            ["title"] = error.Title,
            ["statusCode"] = error.GetHttpStatusCode().ToString(),
            ["errorType"] = error.Type,
            ["timestamp"] = error.Timestamp.ToString("dd-MM-yyyTHH:mm:ss.fff"),
            ["details"] = string.Join(Environment.NewLine, error.Details),
            ["metadata"] = string.Join(Environment.NewLine, error.Metadata.Select(x => StringExtensions.GetStringOrEmpty(x.Key, x.Value)).ToArray()),
            ["stackTraces"] = string.Join(Environment.NewLine, error.StackTraces)
        }))
        {
            logger.LogError(error.Message);
        }
    }
}
