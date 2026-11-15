namespace DebridLinkSharp.Models.Downloader;

// The API sends the limits of a host as a pair: what is used now (current) and the limit (value).
public sealed class Limit
{
    public long Current { get; init; }

    public long Value { get; init; }
}
