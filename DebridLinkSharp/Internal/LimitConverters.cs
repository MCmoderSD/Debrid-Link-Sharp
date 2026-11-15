using System.Text.Json;
using System.Text.Json.Serialization;

namespace DebridLinkSharp.Internal;

// The API sends some limits as { "current": ..., "value": ... }. These converters take the one number that matters out of that object.
internal static class CurrentValuePair
{
    public static (double? Current, double? Value) Read(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("A limit must be an object with the numbers current and value.");
        }

        double? current = null;
        double? value = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return (current, value);
            }

            if (reader.ValueTextEquals("current"))
            {
                reader.Read();
                current = ReadNumber(ref reader);
            }
            else if (reader.ValueTextEquals("value"))
            {
                reader.Read();
                value = ReadNumber(ref reader);
            }
            else
            {
                // Another property: skip its value, which can be anything.
                reader.Read();
                reader.Skip();
            }
        }

        throw new JsonException("The limit is not complete.");
    }

    private static double ReadNumber(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetDouble(out var number))
        {
            throw new JsonException("The numbers of a limit must be numbers.");
        }

        return number;
    }
}

// usagePercent: how much of the daily limit is used, from 0 to 100. The API also sends the 100 as "value", which is not needed.
internal sealed class UsagePercentConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return CurrentValuePair.Read(ref reader).Current ?? 0;
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("current", value);
        writer.WriteNumber("value", 100);
        writer.WriteEndObject();
    }
}

// nextResetSeconds: the time until the next reset of the daily limits, as the number of seconds in "value".
// The API also sends "current" (-1 in the example of the documentation), which is not used.
internal sealed class ResetPeriodConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var seconds = CurrentValuePair.Read(ref reader).Value ?? 0;

        try
        {
            return TimeSpan.FromSeconds(seconds);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or OverflowException)
        {
            throw new JsonException("The time until the next reset is outside the range that can be represented.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("current", -1);
        writer.WriteNumber("value", (long)value.TotalSeconds);
        writer.WriteEndObject();
    }
}
