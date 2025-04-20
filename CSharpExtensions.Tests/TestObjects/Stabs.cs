using CSharpExtensions.Lib.Exceptions;
using CSharpExtensions.Lib.Railway;

namespace CSharpExtensions.Tests.TestObjects;

public static class Stabs
{
    public static Error MyTestError()
    {
        var ex = new InternalServerException("Произошла какая-то неведомая дичь!");

        var error = new Error("Test Message")
            .AsInternalServer("TestMessageError", "Уж случилось так случилось.")
            .WithDetails("Ох бабаньки, штошь делаетси.")
            .WithMetadata("TestKeyMetadata", "TestValueMetadata")
            .CausedBy(ex);

        return error;
    }
	
	public static Result TestResult()
    {
        return MyTestError();
    }

    public static Result<string> TestResultT()
    {
        return MyTestError();
    }

    public static string BigProblemDetailsAsText = @"
    {
        ""title"": ""Произошла системная ошибка."",
        ""type"": ""InternalSystemError"",
        ""status"": 500,
        ""instance"": ""POST /cbonds/get-branches"",
        ""detail"": ""Какое-то тестовое сообщение."",
        ""timestamp"": ""2024-08-13T11:13:40.7987013+03:00"",
        ""traceId"": ""00-c74fd812c3822abfcf97eb008518ca81-f20f620a50427d49-00"",
        ""requestId"": ""00000000-0000-0000-0000-000000000001"",
        ""version"": ""1""
        }
    }";
}
