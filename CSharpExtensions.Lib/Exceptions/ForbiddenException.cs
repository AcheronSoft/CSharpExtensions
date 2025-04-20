namespace CSharpExtensions.Lib.Exceptions;

/// <summary>
/// Клиентское исключение, где пользователю отказано в доступе, статус код 403
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public ForbiddenException()
    {
    }

    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public ForbiddenException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public ForbiddenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
