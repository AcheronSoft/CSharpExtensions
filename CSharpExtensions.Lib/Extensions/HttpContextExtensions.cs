namespace CSharpExtensions.Lib.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Получение версии Api из значения ContentType
    /// </summary>
    public static string GetVersionOfApiFromContentType(string? contentType)
    {
        if(StringExtensions.IsNullOrEmptyOrWhiteSpace(contentType))
            return string.Empty;

        if(!contentType!.Contains("v="))
            return string.Empty;

        var startIndex = contentType.IndexOf("v=", StringComparison.Ordinal) + 2;

        var array = contentType.ToCharArray();
        var version = "";

        for(var i = startIndex; i < array.Length; i++)
        {
            if(array[i] == ' ' || array[i] == ';')
                break;
            version += array[i];
        }

        return version;
    }
}
