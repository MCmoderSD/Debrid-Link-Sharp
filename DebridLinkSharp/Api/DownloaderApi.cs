using DebridLinkSharp.Internal;
using DebridLinkSharp.Models.Downloader;

namespace DebridLinkSharp.Api;

public sealed class DownloaderApi
{
    private readonly DebridLinkHttp _http;

    internal DownloaderApi(DebridLinkHttp http)
    {
        _http = http;
    }

    public async Task<DownloaderLimits> GetLimitsAsync(bool all = false, CancellationToken cancellationToken = default)
    {
        var path = all ? "downloader/limits/all" : "downloader/limits";
        var value = await _http.SendAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);
        return DebridLinkHttp.Deserialize(value, DebridLinkJsonContext.Default.DownloaderLimits, "the limits");
    }
}
