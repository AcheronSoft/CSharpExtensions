namespace CSharpExtensions.Lib.Exceptions.Static;

public static class Exceptions
{

    /// <summary>
    /// Клиентское исключение. Любое исключение со статус кодом 400-499
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static BadRequestException BadRequest(string message, string errorType, string errorTitle)
        => new BadRequestException(message, errorType, errorTitle);


    /// <summary>
    /// Клиентское исключение. Любое исключение со статус кодом 400-499
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static BadRequestException BadRequest(string message, string errorType, string errorTitle, Exception? innerException)
        => new BadRequestException(message, errorType, errorTitle, innerException);

    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public static ForbiddenException Forbidden() => new();

    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public static ForbiddenException Forbidden(string? message) => new(message);

    /// <summary>
    /// Клиентское исключение, где пользователю отказано в доступе, статус код 403
    /// </summary>
    public static ForbiddenException Forbidden(string? message, Exception? innerException) => new(message, innerException);

    /// <summary>
    /// Клиентское исключение, где объект или ресурс был не найден, статус код 404
    /// </summary>
    public static NotFoundException NotFound(string message) => new(message);

    /// <summary>
    /// Клиентское исключение, где объект или ресурс был не найден, статус код 404
    /// </summary>
    public static NotFoundException NotFound(string message, Exception? innerException) => new(message, innerException);

    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public static UnauthorizedException Unauthorized() => new();

    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public static UnauthorizedException Unauthorized(string? message) => new(message);

    /// <summary>
    /// Клиентское исключение, где пользователь был не авторизован, статус код 401
    /// </summary>
    public static UnauthorizedException Unauthorized(string? message, Exception? innerException) => new(message, innerException);

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static InternalServerException InternalServer(string message) => new InternalServerException(message);

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static InternalServerException InternalServer(string message, Exception? innerException) => new InternalServerException(message, innerException);

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static InternalServerException InternalServer(string message, string errorType, string errorTitle)
        => new InternalServerException(message, errorType, errorTitle);

    /// <summary>
    /// Серверное исключение. Любое исключение со статус кодом 500-599
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="errorType">Тип ошибки.</param>
    /// <param name="errorTitle">Заголовок ошибки.</param>
    /// <param name="innerException">Внутренее исключение.</param>
    /// <remarks><para>После того как <b>ErrorType</b> создан, его необходимо занести в реестр кастомных ошибок.</para></remarks>
    /// <exception cref="ArgumentException">Если отсутствуют обязательные параметры.</exception>
    /// </summary>
    public static InternalServerException InternalServer(string message, string errorType, string errorTitle, Exception? innerException)
        => new InternalServerException(message, errorType, errorTitle, innerException);
}
