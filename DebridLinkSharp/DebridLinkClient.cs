using DebridLinkSharp.Api;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp;

public sealed class DebridLinkClient
{
    private static readonly HttpClient SharedClient = new() { Timeout = Timeout.InfiniteTimeSpan };

    public DebridLinkClient(string apiKey) : this(SharedClient, apiKey) { }

    public DebridLinkClient(string apiKey, DebridLinkClientOptions options) : this(SharedClient, apiKey, options ?? throw new ArgumentNullException(nameof(options))) { }

    public DebridLinkClient(HttpClient httpClient, string apiKey, DebridLinkClientOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        var http = new DebridLinkHttp(httpClient, apiKey.Trim(), (options ?? new DebridLinkClientOptions()).CloneAndValidate());
        
        Account = new AccountApi(http);
        Downloader = new DownloaderApi(http);
    }

    public AccountApi Account { get; }

    public DownloaderApi Downloader { get; }
}