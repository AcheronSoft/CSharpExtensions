namespace CSharpExtensions.Lib.Railway;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Если результат успешен, он не должен содержать объект ошибки.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Если результат неуспешен, он должен содержать объект ошибки.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result Failure(string message) => new(false, new Error(message));

    public static Result<TValue> Failure<TValue>(string message) => new(default, false, new Error(message));

    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(new Error($"Ожидаемое значение {typeof(TValue).Name} оказалось null."));

    public static implicit operator Result(Error error) => Failure(error);
}
