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

    public async Task<IReadOnlyList<DownloaderHost>> GetHostsAsync(CancellationToken cancellationToken = default)
    {
        var value = await _http.SendAsync(HttpMethod.Get, "downloader/hosts", null, cancellationToken).ConfigureAwait(false);
        var hosts = DebridLinkHttp.Deserialize(value, DebridLinkJsonContext.Default.DownloaderHostArray, "a list of hosts");
        DebridLinkHttp.RequireNoNulls(hosts, "a list of hosts");
        return hosts;
    }

    public async Task<DownloaderLimits> GetLimitsAsync(bool all = false, CancellationToken cancellationToken = default)
    {
        var path = all ? "downloader/limits/all" : "downloader/limits";
        var value = await _http.SendAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);
        return DebridLinkHttp.Deserialize(value, DebridLinkJsonContext.Default.DownloaderLimits, "the limits");
    }
}
