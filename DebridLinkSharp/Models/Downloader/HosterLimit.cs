using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class HosterLimit
{
    [JsonPropertyName("name")]
    [JsonConverter(typeof(HosterConverter))]
    public Hoster Hoster { get; init; }

    // Bytes per day.
    public Limit DaySize { get; init; } = new();

    // Links per day.
    public Limit DayCount { get; init; } = new();
}