using CSharpExtensions.Lib.Extensions;
using Microsoft.AspNetCore.Http;

namespace CSharpExtensions.Lib.Railway;

/// <summary>
/// Класс олицетворяющий объект ошибки
/// </summary>
public record Error
{
    private Error()
    {
        Timestamp = DateTime.MinValue;
        Metadata = new Dictionary<string, object>();
        Details = new List<string>();
        StackTraces = new List<string>();
    }

    public Error(string message) : this()
    {
        if(StringExtensions.IsNullOrEmptyOrWhiteSpace(message))
            throw new ArgumentException($"При регистрации ошибки вы не указали сообщение об ошибке - [{nameof(Message)}].");
        Message = message;

        // По умолчанию созданная ошибка будет серверной
        Timestamp = DateTime.Now;
        Type = "InternalServerError";
        Title = "Произошла серверная ошибка.";
        HttpStatusCode = StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Заголовок ошибки
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Тип ошибки. Правила наименования:
    /// <list type="bullet">
    /// <item>Наименование начинается с заглавной буквы</item>
    /// <item>Стиль CamelCase</item>
    /// <item>Заканчивается наименование на слово 'Error'.</item>
    /// </list>
    /// Пример: MyAwesomeError
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Где произошла ошибка - на клиенте или на сервере
    /// </summary>
    private int HttpStatusCode { get; set; }

    /// <summary>
    /// Время ошибки
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Дополнительные реквизиты ошибки
    /// </summary>
    public Dictionary<string, object> Metadata { get; }

    /// <summary>
    /// Список причин ошибки
    /// </summary>
    public List<string> Details { get; }

    /// <summary>
    /// Стек трейс ошибки
    /// </summary>
    public List<string> StackTraces { get; }

    /// <summary>
    /// Преобразовывает ошибку в BadRequestError
    /// </summary>
    /// <param name="type">Тип ошибки - <b>обязательное поле</b> </param>
    /// <param name="title">Заголовок ошибки - <b>обязательное поле</b> </param>
    /// <example><b>Пример Type:</b> AccountClosedError</example>
    /// <example><b>Пример Title:</b> Ошибка обращения к закрытому счету.</example>
    /// <exception cref="ArgumentException">Если type/title пустой или null</exception>
    public Error AsBadRequest(string type, string title)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(type))
            throw new ArgumentException("При регистрации ошибки вы не указали тип - [ErrorType].");

        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(title))
            throw new ArgumentException("При регистрации ошибки вы не указали заголовок - [ErrorTitle].");

        Type = type;
        Title = title;
        HttpStatusCode = StatusCodes.Status400BadRequest;

        return this;
    }

    /// <summary>
    /// Преобразовывает ошибку в UnauthorizedError
    /// </summary>
    public Error AsUnauthorized()
    {
        Type = "UnauthorizedError";
        Title = "Произошла ошибка авторизации.";
        HttpStatusCode = StatusCodes.Status401Unauthorized;

        return this;
    }

    /// <summary>
    /// Преобразовывает ошибку в ForbiddenError
    /// </summary>
    public Error AsForbidden()
    {
        Type = "ForbiddenError";
        Title = "Произошла ошибка отказа в доступе.";
        HttpStatusCode = StatusCodes.Status403Forbidden;

        return this;
    }

    /// <summary>
    /// Преобразовывает ошибку в NotFoundError
    /// </summary>
    public Error AsNotFound()
    {
        Type = "NotFoundError";
        Title = "По указанному маршруту данные не найдены.";
        HttpStatusCode = StatusCodes.Status404NotFound;

        return this;
    }

    /// <summary>
    /// Преобразовывает ошибку в InternalServerError
    /// </summary>
    /// <param name="type">Тип ошибки</param>
    /// <param name="title">Заголовок ошибки</param>
    /// <example><b>Пример Type:</b> AccountClosedError</example>
    /// <example><b>Пример Title:</b> Ошибка обращения к закрытому счету.</example>
    /// <exception cref="ArgumentException">Если type/title пустой или null</exception>
    public Error AsInternalServer(string type, string title)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(type) && StringExtensions.IsNullOrEmptyOrWhiteSpace(title))
            throw new ArgumentException("При регистрации ошибки вы не указали тип и заголовок ошибки - [ErrorType]/[ErrorTitle].");

        Type = type;
        Title = title;
        HttpStatusCode = StatusCodes.Status500InternalServerError;

        return this;
    }

    /// <summary>
    /// Добавляет причину ошибки
    /// </summary>
    public Error WithDetails(string details)
    {
        if (!StringExtensions.IsNullOrEmptyOrWhiteSpace(details) && !Details.Contains(details) && Message != details)
            Details.Add(details);
        return this;
    }

    public int GetHttpStatusCode()
    {
        return HttpStatusCode;
    }

    /// <summary>
    /// Добавляет StackTrace
    /// </summary>
    private void WithStackTrace(string stackTrace)
    {
        if (!StringExtensions.IsNullOrEmptyOrWhiteSpace(stackTrace) && !StackTraces.Contains(stackTrace))
            StackTraces.Add(stackTrace);
    }

    /// <summary>
    /// Заменяет предустановленный при инициализации Timestamp
    /// </summary>
    public Error WithTimestamp(DateTime timestamp)
    {
        Timestamp = timestamp;
        return this;
    }

    /// <summary>
    /// Добавляет причину ошибки в виде исключения
    /// </summary>
    public Error CausedBy(Exception exception)
    {
        if (exception == null)
            throw new ArgumentNullException($"В методе расширении {nameof(CausedBy)} входящий параметр {nameof(exception)} был null.");

        foreach (var message in exception.GetMessages())
        {
            WithDetails(message);
        }

        foreach (var stackTrace in exception.GetStackTrace())
        {
            WithStackTrace(stackTrace);
        }

        return this;
    }

    /// <summary>
    /// Добавляет реквизиты ошибки
    /// </summary>
    public Error WithMetadata(string metadataName, object metadataValue)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(metadataName))
            throw new ArgumentException($"В методе расширении {nameof(WithMetadata)} входящий параметр {nameof(metadataName)} был пустым или null.");

        if(metadataValue == null)
            throw new ArgumentNullException($"В методе расширении {nameof(WithMetadata)} входящий параметр {nameof(metadataValue)} был null.");

        if (Metadata.ContainsKey(metadataName))
            Metadata[metadataName] = metadataValue;
        else
            Metadata.Add(metadataName, metadataValue);
        return this;
    }

    /// <summary>
    /// Добавляет реквизиты ошибки
    /// </summary>
    public Error WithMetadata(Dictionary<string, object> metadata)
    {
        foreach (var metadataItem in metadata)
            WithMetadata(metadataItem.Key, metadataItem.Value);

        return this;
    }

    public bool HasMetadataKey(string key)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(key))
            throw new ArgumentException($"В методе расширении {nameof(HasMetadataKey)} входящий параметр {nameof(key)} был пустым или null.");

        return Metadata.ContainsKey(key);
    }

    public bool HasMetadata(string key, Func<object, bool> predicate)
    {
        if (StringExtensions.IsNullOrEmptyOrWhiteSpace(key))
            throw new ArgumentException($"В методе расширении {nameof(HasMetadata)} входящий параметр {nameof(key)} был пустым или null.");

        if (predicate == null)
            throw new ArgumentNullException($"В методе расширении {nameof(HasMetadata)} входящий параметр {nameof(predicate)} был null.");

        if (Metadata.TryGetValue(key, out object actualValue))
            return predicate(actualValue);

        return false;
    }

    /// <summary>
    /// Дефолтное значение для пустой ошибки чтобы не передавать null
    /// </summary>
    public static Error None => new Error();

    /// <summary>
    /// Концептуально при сравнении двух ошибок мы не учитываем время ошибки,
    /// если нужно сравнение полное, включите в условие сравнение полей Timestamp
    /// </summary>
    public virtual bool Equals(Error? other)
    {
        if (other == null)
            return false;

        return Message == other.Message &&
            Type == other.Type &&
            Title == other.Title &&
            HttpStatusCode == other.GetHttpStatusCode() &&
            Details.SequenceEqual(other.Details) &&
            Metadata.SequenceEqual(other.Metadata);
    }

    /// <summary>
    /// Для получения хеш-суммы не учитываается время ошибки
    /// </summary>
    public override int GetHashCode()
    {
        var message = Message ?? "";
        var type = Type ?? "";
        var title = Title ?? "";
        var statusCode = GetHttpStatusCode();
        var details = string.Join(',', Details);
        var metadata = string.Join(',', Metadata);

        var data = message + "_" + type + "_" + title + "_" + statusCode + "_" + details + "_" + metadata;

        return data.GetHashCode();
    }
}
