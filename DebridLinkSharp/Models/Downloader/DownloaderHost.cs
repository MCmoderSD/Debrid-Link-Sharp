using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderHost
{
    public string Name { get; init; } = "";

    [JsonIgnore]
    public Hoster Hoster => HosterNames.Parse(Name);

    [JsonConverter(typeof(HostTypeConverter))]
    public HostType Type { get; init; }

    public int Status { get; init; }

    public bool IsFree { get; init; }

    public IReadOnlyList<string> Domains { get; init; } = [];

    public IReadOnlyList<string> Regexs { get; init; } = [];

    [JsonIgnore]
    public bool IsOnline => Status >= 1;
}