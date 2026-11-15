using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Account;

public sealed class AccountInfo
{
    public string Username { get; init; } = "";

    public string Email { get; init; } = "";

    public bool EmailVerified { get; init; }

    public AccountType AccountType { get; init; }

    [JsonConverter(typeof(DurationSecondsConverter))]
    public TimeSpan PremiumLeft { get; init; }

    [JsonPropertyName("pts")]
    public long Points { get; init; }

    public string UpgradeAccountUrl { get; init; } = "https://debrid-link.com/premium";

    public DateOnly RegisterDate { get; init; }

    public bool ServerDetected { get; init; }

    public AccountSettings Settings { get; init; } = new();

    public string AvatarUrl { get; init; } = "";
}