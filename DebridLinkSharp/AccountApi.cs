using DebridLinkSharp.Internal;
using DebridLinkSharp.Models.Account;

namespace DebridLinkSharp;

public sealed class AccountApi
{
    private readonly DebridLinkHttp _http;

    internal AccountApi(DebridLinkHttp http)
    {
        _http = http;
    }

    public async Task<AccountInfo> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        var value = await _http.SendAsync(HttpMethod.Get, "account/infos", null, cancellationToken).ConfigureAwait(false);
        return DebridLinkHttp.Deserialize(value, DebridLinkJsonContext.Default.AccountInfo, "account information");
    }
}