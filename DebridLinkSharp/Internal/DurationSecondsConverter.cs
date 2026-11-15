using System.Text.Json;
using System.Text.Json.Serialization;

namespace DebridLinkSharp.Internal;

internal sealed class DurationSecondsConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetInt64(out var seconds))
        {
            throw new JsonException("A duration must be a whole number of seconds.");
        }

        try
        {
            return TimeSpan.FromSeconds(seconds);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or OverflowException)
        {
            throw new JsonException("The duration is outside the range that can be represented.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((long)value.TotalSeconds);
    }
}