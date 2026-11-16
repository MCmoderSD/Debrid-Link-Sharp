using System.Text.Json;
using DebridLinkSharp.Internal;
using DebridLinkSharp.Models;
using DebridLinkSharp.Models.Downloader;

namespace DebridLinkSharp.Api;

public sealed class DownloaderApi
{
    private readonly DebridLinkHttp _http;

    internal DownloaderApi(DebridLinkHttp http)
    {
        _http = http;
    }

    public Task<IReadOnlyList<DownloaderLink>> AddAsync(string url, string? password = null, string? ip = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        return AddCoreAsync(new DownloaderAddRequest { Url = url, Password = password, Ip = ip }, cancellationToken);
    }

    public Task<PagedResult<DownloaderLink>> ListAsync(int? page = null, int? perPage = null, string? ip = null, IEnumerable<string>? ids = null, CancellationToken cancellationToken = default)
    {
        if (page is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "The first page is 0.");
        }

        if (perPage is < 20 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(perPage), perPage, "The number of items per page must be between 20 and 100.");
        }

        if (ip != null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ip);
        }

        var idList = ids == null ? null : CheckIds(ids, nameof(ids), 100);
        var path = QueryString.Append("downloader/list", QueryString.Pair("page", page), QueryString.Pair("perPage", perPage), QueryString.Pair("ip", ip), QueryString.Pair("ids", idList));
        return ListCoreAsync(path, cancellationToken);
    }

    public Task<IReadOnlyList<string>> RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return RemoveCoreAsync([id], cancellationToken);
    }

    public Task<IReadOnlyList<string>> RemoveAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        return RemoveCoreAsync(CheckIds(ids, nameof(ids), null), cancellationToken);
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

    private async Task<IReadOnlyList<DownloaderLink>> AddCoreAsync(DownloaderAddRequest request, CancellationToken cancellationToken)
    {
        var content = JsonContent.Create(request, DebridLinkJsonContext.Default.DownloaderAddRequest);
        var value = await _http.SendAsync(HttpMethod.Post, "downloader/add", content, cancellationToken).ConfigureAwait(false);
        return ReadLinks(value);
    }

    private async Task<PagedResult<DownloaderLink>> ListCoreAsync(string path, CancellationToken cancellationToken)
    {
        var answer = await _http.SendPagedAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);
        var links = DebridLinkHttp.Deserialize(answer.Value, DebridLinkJsonContext.Default.DownloaderLinkArray, "a list of links");
        DebridLinkHttp.RequireNoNulls(links, "a list of links");

        // Without a pagination object, everything is taken as one page.
        var pagination = answer.Pagination is { } element
            ? DebridLinkHttp.Deserialize(element, DebridLinkJsonContext.Default.PaginationDto, "the pagination")
            : new PaginationDto();

        return new PagedResult<DownloaderLink> { Items = links, Page = pagination.Page, Pages = pagination.Pages, Next = pagination.Next, Previous = pagination.Previous };
    }

    private async Task<IReadOnlyList<string>> RemoveCoreAsync(List<string> ids, CancellationToken cancellationToken)
    {
        var path = $"downloader/{string.Join(',', ids.Select(Uri.EscapeDataString))}/remove";
        var value = await _http.SendAsync(HttpMethod.Delete, path, null, cancellationToken).ConfigureAwait(false);
        var removed = DebridLinkHttp.Deserialize(value, DebridLinkJsonContext.Default.StringArray, "a list of ids");
        DebridLinkHttp.RequireNoNulls(removed, "a list of ids");
        return removed;
    }

    private static List<string> CheckIds(IEnumerable<string> ids, string parameter, int? maximum)
    {
        ArgumentNullException.ThrowIfNull(ids, parameter);

        var list = ids.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Pass at least one id.", parameter);
        }

        if (list.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("An id must not be empty.", parameter);
        }

        if (maximum is { } max && list.Count > max)
        {
            throw new ArgumentException($"At most {max} ids are allowed.", parameter);
        }

        return list;
    }

    private static IReadOnlyList<DownloaderLink> ReadLinks(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                return [ReadLink(value)];

            case JsonValueKind.Array:
                var links = new List<DownloaderLink>(capacity: value.GetArrayLength());
                foreach (var item in value.EnumerateArray())
                {
                    links.Add(ReadLink(item));
                }

                return links;

            default:
                throw new DebridLinkServiceException(DebridLinkServiceError.InvalidResponse, $"Debrid-Link answered with {value.ValueKind} where a link or a list of links was expected.");
        }
    }

    private static DownloaderLink ReadLink(JsonElement element)
    {
        return DebridLinkHttp.Deserialize(element, DebridLinkJsonContext.Default.DownloaderLink, "a link");
    }
}
