using System.Text.Json;
using System.Text.Json.Serialization;
using DebridLinkSharp.Models.Account;

namespace DebridLinkSharp.Internal;

internal abstract class WireEnumConverter<T>((string Wire, T Value)[] map) : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var wire = reader.GetString();
            foreach (var (name, value) in map)
            {
                if (string.Equals(name, wire, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
        }

        reader.Skip();
        return default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        foreach (var (name, known) in map)
        {
            if (!EqualityComparer<T>.Default.Equals(known, value)) continue;
            writer.WriteStringValue(name);
            return;
        }

        writer.WriteNullValue();
    }
}

internal sealed class CdnConverter() : WireEnumConverter<Cdn>([("auto", Cdn.Auto), ("direct", Cdn.Direct), ("cloudflare", Cdn.Cloudflare), ("scaleway", Cdn.Scaleway)]);

internal sealed class IpVersionConverter() : WireEnumConverter<IpVersion>([("auto", IpVersion.Auto), ("v4", IpVersion.IPv4)]);

internal sealed class TwoFactorTypeConverter() : WireEnumConverter<TwoFactorType>([("email", TwoFactorType.Email), ("otp", TwoFactorType.Otp)]);