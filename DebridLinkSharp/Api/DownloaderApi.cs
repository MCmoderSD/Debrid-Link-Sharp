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

    public Task<IReadOnlyList<DownloaderHost>> GetHostsAsync(IEnumerable<HostType>? types = null, CancellationToken cancellationToken = default)
    {
        List<string>? names = null;

        if (types != null)
        {
            names = [];
            foreach (var type in types)
            {
                if (type == HostType.Unknown || !Enum.IsDefined(type))
                {
                    throw new ArgumentException($"The host type {type} cannot be requested.", nameof(types));
                }

                var name = type.ToString().ToLowerInvariant();
                if (!names.Contains(name))
                {
                    names.Add(name);
                }
            }

            if (names.Count == 0)
            {
                throw new ArgumentException("Pass at least one host type, or null for all of them.", nameof(types));
            }
        }

        return GetHostsCoreAsync(QueryString.Append("downloader/hosts", QueryString.Pair("types", names)), cancellationToken);
    }

    private async Task<IReadOnlyList<DownloaderHost>> GetHostsCoreAsync(string path, CancellationToken cancellationToken)
    {
        var value = await _http.SendAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);
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