using CSharpExtensions.Lib.Extensions;

namespace CSharpExtensions.Lib.Exceptions;

/// <summary>
/// Клиентское исключение. Любое исключение со статус кодом 400
/// </summary>
public class BadRequestException : Exception
{
    public string ErrorType { get; }

    public string ErrorTitle { get; }


    /// <summary>
    /// Клиентское исключение. Любое исключение со статус кодом 400-499
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public BadRequestException(string message, string errorType, string errorTitle) : base(message)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(BadRequestException)} вы не указали сообщение об ошибке - [{nameof(message)}].");

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(errorType) && StringExtensions.IsNullOrEmptyOrWhiteSpace(errorTitle))
            throw new ArgumentException($"При генерации исключения {nameof(BadRequestException)} вы не указали тип или заголовок ошибки - [{nameof(errorType)}] или [{nameof(errorTitle)}].");

        ErrorType = errorType;
        ErrorTitle = errorTitle;
    }

    /// <summary>
    /// Клиентское исключение. Любое исключение со статус кодом 400-499
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public BadRequestException(string message, string errorType, string errorTitle, Exception? innerException) : base(message, innerException)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(BadRequestException)} вы не указали сообщение об ошибке - [{nameof(message)}].");

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(errorType) && StringExtensions.IsNullOrEmptyOrWhiteSpace(errorTitle))
            throw new ArgumentException($"При генерации исключения {nameof(BadRequestException)} вы не указали тип или заголовок ошибки - [{nameof(errorType)}] или [{nameof(errorTitle)}].");

        ErrorType = errorType;
        ErrorTitle = errorTitle;
    }
}
