using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderHost
{
    public string Name { get; init; } = "";

    // The same host as the Hoster enum. It is Unknown if the enum does not have the host, which is the case for every streaming site and for some file hosts.
    [JsonIgnore]
    public Hoster Hoster => HosterNames.Parse(Name);

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
