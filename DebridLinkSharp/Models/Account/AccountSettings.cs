using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Account;

public sealed class AccountSettings
{
    public bool Https { get; init; }

    public bool ThemeDark { get; init; }

    public bool HideOldLinks { get; init; }

    [JsonConverter(typeof(CdnConverter))]
    public Cdn Cdn { get; init; }

    [JsonPropertyName("twofaType")]
    [JsonConverter(typeof(TwoFactorTypeConverter))]
    public TwoFactorType TwoFactorType { get; init; }

    public bool EmailsNews { get; init; }

    public bool EmailsAccount { get; init; }

    public bool EmailsSupport { get; init; }

    [JsonConverter(typeof(IpVersionConverter))]
    public IpVersion IpVersion { get; init; }
}