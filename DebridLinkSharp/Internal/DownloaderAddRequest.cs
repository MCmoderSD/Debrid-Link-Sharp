namespace DebridLinkSharp.Internal;

internal sealed class DownloaderAddRequest
{
    public required string Url { get; init; }

    public string? Password { get; init; }

    public string? Ip { get; init; }
}