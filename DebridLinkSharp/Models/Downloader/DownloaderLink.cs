using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderLink
{
    public string Id { get; init; } = "";

    public string? Name { get; init; }

    public string? Url { get; init; }

    public string? DownloadUrl { get; init; }

    public string? Host { get; init; }

    public long Size { get; init; }

    public int? Chunk { get; init; }

    [JsonConverter(typeof(UnixSecondsConverter))]
    public DateTimeOffset Created { get; init; }

    public bool Expired { get; init; }

    public bool IsProcessing { get; init; }
}
