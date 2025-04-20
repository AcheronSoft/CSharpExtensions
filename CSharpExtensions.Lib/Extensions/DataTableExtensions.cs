using System.Data;
using System.Text.Json;

namespace CSharpExtensions.Lib.Extensions;

public static class DataTableExtensions
{
    public static DataTable JsonElementToDataTable(this JsonElement dataRoot)
    {
        var dataTable = new DataTable();
        bool firstPass = true;
        foreach (JsonElement element in dataRoot.EnumerateArray())
        {
            DataRow row = dataTable.NewRow();
            dataTable.Rows.Add(row);
            foreach (JsonProperty col in element.EnumerateObject())
            {
                if (firstPass)
                {
                    JsonElement colValue = col.Value;
                    dataTable.Columns.Add(new DataColumn(col.Name, colValue.ValueKind.ValueKindToType(colValue.ToString()!)));
                }
                row[col.Name] = col.Value.JsonElementToTypedValue();
            }
            firstPass = false;
        }

        return dataTable;
    }

    private static Type ValueKindToType(this JsonValueKind valueKind, string value)
    {
        switch (valueKind)
        {
            case JsonValueKind.String:
                return typeof(string);
            case JsonValueKind.Number:
                if (long.TryParse(value, out _))
                {
                    return typeof(long);
                }
                else
                {
                    return typeof(double);
                }
            case JsonValueKind.True:
            case JsonValueKind.False:
                return typeof(bool);
            case JsonValueKind.Undefined:
                throw new NotSupportedException();
            case JsonValueKind.Object:
                return typeof(object);
            case JsonValueKind.Array:
                return typeof(System.Array);
            case JsonValueKind.Null:
                throw new NotSupportedException();
            default:
                return typeof(object);
        }
    }

    private static object? JsonElementToTypedValue(this JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                throw new NotSupportedException();
            case JsonValueKind.String:
                if (jsonElement.TryGetGuid(out Guid guidValue))
                {
                    return guidValue;
                }

                if (jsonElement.TryGetDateTime(out DateTime datetime))
                {
                    if (datetime.Kind == DateTimeKind.Local)
                    {
                        if (jsonElement.TryGetDateTimeOffset(out DateTimeOffset datetimeOffset))
                        {
                            return datetimeOffset;
                        }
                    }
                    return datetime;
                }
                return jsonElement.ToString();
            case JsonValueKind.Number:
                if (jsonElement.TryGetInt64(out long longValue))
                    return longValue;
                return jsonElement.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return jsonElement.GetBoolean();
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            default:
                return jsonElement.ToString();
        }
    }
}
