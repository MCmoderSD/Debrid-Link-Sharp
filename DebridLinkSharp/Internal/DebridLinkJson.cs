using System.Text.Json.Serialization;
using DebridLinkSharp.Models.Account;

namespace DebridLinkSharp.Internal;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
)]

[JsonSerializable(typeof(AccountInfo))]
[JsonSerializable(typeof(Cdn))]
[JsonSerializable(typeof(IpVersion))]
[JsonSerializable(typeof(TwoFactorType))]
internal sealed partial class DebridLinkJsonContext : JsonSerializerContext;