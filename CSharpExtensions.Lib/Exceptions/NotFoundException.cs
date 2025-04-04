using CSharpExtensions.Lib.Extensions;

namespace CSharpExtensions.Lib.Exceptions;

/// <summary>
/// Клиентское исключение, где объект или ресурс был не найден, статус код 404
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Клиентское исключение, где объект или ресурс был не найден, статус код 404
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public NotFoundException(string message) : base(message)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(NotFoundException)} вы не указали сообщение об ошибке - [{nameof(message)}].");
    }

    /// <summary>
    /// Клиентское исключение, где объект или ресурс был не найден, статус код 404
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public NotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(NotFoundException)} вы не указали сообщение об ошибке - [{nameof(message)}].");
    }
}
