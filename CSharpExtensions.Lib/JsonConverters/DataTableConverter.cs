using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpExtensions.Lib.Extensions;

namespace CSharpExtensions.Lib.JsonConverters;

public class DataTableConverter : JsonConverter<DataTable>
{
    public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDoc.RootElement;
        var dataTable = rootElement.JsonElementToDataTable();
        return dataTable;
    }

    public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (DataRow row in value.Rows)
        {
            writer.WriteStartObject();
            foreach (DataColumn column in value.Columns)
            {
                var key = column.ColumnName.Trim();

                var action = GetWriteAction(row, column, writer);
                action.Invoke(key);

                static Action<string> GetWriteAction(
                    DataRow row, DataColumn column, Utf8JsonWriter writer) => row[column] switch
                    {
                        // bool
                        bool value => key => writer.WriteBoolean(key, value),

                        // numbers
                        byte value => key => writer.WriteNumber(key, value),
                        sbyte value => key => writer.WriteNumber(key, value),
                        decimal value => key => writer.WriteNumber(key, value),
                        double value => key => writer.WriteNumber(key, value),
                        float value => key => writer.WriteNumber(key, value),
                        short value => key => writer.WriteNumber(key, value),
                        int value => key => writer.WriteNumber(key, value),
                        ushort value => key => writer.WriteNumber(key, value),
                        uint value => key => writer.WriteNumber(key, value),
                        ulong value => key => writer.WriteNumber(key, value),

                        // strings
                        DateTime value => key => writer.WriteString(key, value),
                        Guid value => key => writer.WriteString(key, value),

                        _ => key => writer.WriteString(key, row[column].ToString())
                    };
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}

