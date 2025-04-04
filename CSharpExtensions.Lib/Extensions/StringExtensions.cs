namespace CSharpExtensions.Lib.Extensions;

public static class StringExtensions
{
    private const string Line = "/";

    public static bool IsNullOrEmptyOrWhiteSpace(string? text)
    {
        var result1 = string.IsNullOrEmpty(text) ? 1 : 0;
        var result2 = string.IsNullOrWhiteSpace(text) ? 1 : 0;

        return result1 + result2 != 0;
    }

    /// <summary>
    /// Возращает указанное количество знаков с начала строки
    /// </summary>
    /// <param name="text">Строка</param>
    /// <param name="length">Количество знаков</param>
    public static string Left(this string text, int length)
    {
        if (IsNullOrEmptyOrWhiteSpace(text))
            return text;

        length = Math.Abs(length);

        return text.Length <= length ? text : text.Substring(0, length);
    }

    /// <summary>
    /// Возращает указанное количество знаков с конца строки
    /// </summary>
    /// <param name="text">Строка</param>
    /// <param name="length">Количество знаков</param>
    /// <exception cref="ArgumentException"></exception>
    public static string Right(this string text, int length)
    {
        if (IsNullOrEmptyOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if (length < 1)
            throw new ArgumentException($"{nameof(length)} должен быть > 0");

        return text.Length <= length ? text : text[^length..];
    }

    /// <summary>
    /// Добавляет сегмент к строке url
    /// </summary>
    /// <param name="baseAddress">Базовый url</param>
    /// <param name="segment">Добавляемый сегмент</param>
    /// <returns></returns>
    public static Uri AddUrlSegment(this Uri baseAddress, string segment)
    {
        return new Uri(baseAddress, segment);
    }

    /// <summary>
    /// Преобразует ключ-значение в строку {key}={value}
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetStringOrEmpty(string key, object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        var valueText = value.ToJson();

        return valueText == string.Empty ? string.Empty : $"{key}='{valueText}'";
    }

    public static int ToInt(this string text) => int.Parse(text);

    public static DateTime ToDateTime(this string text) => DateTime.Parse(text);

    public static decimal ToDecimal(this string text) => decimal.Parse(text);

    public static bool ToBool(this string text) => bool.Parse(text);

    public static int? ToNullableInt(this string text) =>
        IsNullOrEmptyOrWhiteSpace(text)
            ? null
            : int.TryParse(text, out var value)
                ? value
                : null;

    public static DateTime? ToNullableDateTime(this string text) =>
        IsNullOrEmptyOrWhiteSpace(text)
            ? null
            : DateTime.TryParse(text, out var value)
                ? value
                : null;

    public static decimal? ToNullableDecimal(this string text) =>
        IsNullOrEmptyOrWhiteSpace(text)
            ? null
            : decimal.TryParse(text, out var value)
                ? value
                : null;

    public static bool? ToNullableBool(this string text) =>
        IsNullOrEmptyOrWhiteSpace(text)
            ? null
            : bool.TryParse(text, out var value)
                ? value
                : null;
}
