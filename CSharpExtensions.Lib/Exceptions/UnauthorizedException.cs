namespace CSharpExtensions.Lib.Exceptions;

/// <summary>
/// Клиентское исключение, где пользователь был не авторизован, статус код 401
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public UnauthorizedException()
    {
    }

    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public UnauthorizedException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
