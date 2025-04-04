using CSharpExtensions.Lib.Extensions;

namespace CSharpExtensions.Lib.Exceptions;

/// <summary>
/// Системное исключение. Применяется к:
/// <list type="bullet">
/// <item>К любому неизвестному исключению типа Exception</item>
/// <item>К падению связанному с http статус кодами 500-599</item>
/// </list>
/// </summary>
public class InternalServerException : Exception
{
    public string? ErrorType { get; }

    public string? ErrorTitle { get; }

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public InternalServerException(string message) : base(message)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали сообщение об ошибке - [{nameof(message)}].");
    }

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public InternalServerException(string message, Exception? innerException) : base(message, innerException)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали сообщение об ошибке - [{nameof(message)}].");
    }

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public InternalServerException(string message, string errorType, string errorTitle) : base(message)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали сообщение об ошибке - [{nameof(message)}].");

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(errorType) && StringExtensions.IsNullOrEmptyOrWhiteSpace(errorTitle))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали тип или заголовок ошибки - [{nameof(errorType)}] или [{nameof(errorTitle)}].");

        ErrorType = errorType;
        ErrorTitle = errorTitle;
    }

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public InternalServerException(string message, string errorType, string errorTitle, Exception? innerException) : base(message, innerException)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали сообщение об ошибке - [{nameof(message)}].");

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(errorType) && StringExtensions.IsNullOrEmptyOrWhiteSpace(errorTitle))
            throw new ArgumentException($"При генерации исключения {nameof(InternalServerException)} вы не указали тип или заголовок ошибки - [{nameof(errorType)}] или [{nameof(errorTitle)}].");

        ErrorType = errorType;
        ErrorTitle = errorTitle;
    }
}
