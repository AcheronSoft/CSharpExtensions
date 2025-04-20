using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CSharpExtensions.Lib.JsonConverters;
using CSharpExtensions.Lib.JsonNamingPolicies;

namespace CSharpExtensions.Lib.Extensions;

public static class JsonExtensions
{
    public static readonly JavaScriptEncoder CyrillicEncoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

    public static JsonNamingPolicy LowerCaseNamingPolicy { get; } = new LowerCaseNamingPolicy();

    /// <summary>
    /// Опции Json с игнорированием нулевых значений, включают русско-английскую кодировку и camel case
    /// </summary>
    public static readonly JsonSerializerOptions IgnoreNullJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = CyrillicEncoder,
        Converters = { new DataTableConverter() }
    };

    /// <summary>
    /// Опции Json по умолчанию включают русско-английскую кодировку и camel case
    /// </summary>
    public static readonly JsonSerializerOptions BaseJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = CyrillicEncoder,
        Converters = { new DataTableConverter() }
    };

    /// <summary>
    /// Преобразование объекта в json строку
    /// </summary>
    /// <param name="obj">Объект для преобразования в json строку</param>
    /// <param name="ignoreNullValues">Опция игнорирования нулевых значений. По умолчанию включена.</param>
    public static string ToJson(this object obj, bool ignoreNullValues = true)
    {
        return ignoreNullValues
            ? JsonSerializer.Serialize(obj, IgnoreNullJsonOptions)
            : JsonSerializer.Serialize(obj, BaseJsonOptions);
    }

    /// <summary>
    /// Преобразование объекта в json строку
    /// </summary>
    public static string ToJson(this object obj, Action<JsonSerializerOptions> options)
    {
        var serializerOptions = new JsonSerializerOptions();
        options(serializerOptions);
        return JsonSerializer.Serialize(obj, serializerOptions);
    }

    /// <summary>
    /// Получение http контента в виде строки и десериализация в T
    /// </summary>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }
}
