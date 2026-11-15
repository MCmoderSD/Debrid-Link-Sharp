using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderHost
{
    [JsonPropertyName("name")]
    [JsonConverter(typeof(HosterConverter))]
    public Hoster Hoster { get; init; }

    [JsonConverter(typeof(HostTypeConverter))]
    public HostType Type { get; init; }

    public int Status { get; init; }

    public bool IsFree { get; init; }

    public IReadOnlyList<string> Domains { get; init; } = [];

    public IReadOnlyList<string> Regexs { get; init; } = [];

    // Debrid-Link says that a host is online when its status is 1 or higher.
    [JsonIgnore]
    public bool IsOnline => Status >= 1;
}
