using System.Text.Json;
using System.Text.Json.Serialization;

namespace DebridLinkSharp.Internal;

internal sealed class UnixSecondsConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetInt64(out var seconds))
        {
            throw new JsonException("A point in time must be a whole number of seconds since 1970-01-01.");
        }

        try
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new JsonException("The point in time is outside the range that can be represented.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}