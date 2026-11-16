# DebridLinkSharp

A small .NET client for the **[Debrid-Link](https://debrid-link.com) API v2**. You give it a hoster link and get back the direct download URL, the file name, the size and the host as plain objects. It also reads your account: the account type, the premium time left, the points and the settings, the daily limits and the hosts of the downloader, and it removes links.

- Targets `net11.0` and has no third-party dependencies: HTTP comes from `System.Net.Http`, JSON from `System.Text.Json`.
- Plain model classes. The JSON handling uses serializers generated at compile time, so it needs no reflection.
- Typed errors: the error codes of the API arrive as an exception with an `ErrorCode`.

> **Not affiliated with Debrid-Link.** See [Known limits](#known-limits).

## Installation

```bash
dotnet add package DebridLinkSharp
```

The package targets `net11.0`.

## Quick start

```csharp
using DebridLinkSharp;
using DebridLinkSharp.Models.Account;
using DebridLinkSharp.Models.Downloader;

// A private API key (https://debrid-link.com/webapp/apikey) or an OAuth2 access token.
var client = new DebridLinkClient(apiKey);

AccountInfo account = await client.Account.GetInfoAsync();

Console.WriteLine($"{account.Username}: {account.AccountType}, {account.PremiumLeft.TotalDays:0} days of premium left, {account.Points} points");
```

```csharp
IReadOnlyList<DownloaderLink> links = await client.Downloader.AddAsync("https://hoster.example/file/abc");

foreach (DownloaderLink link in links)
{
    Console.WriteLine($"{link.Name}  {link.Size} bytes  {link.Host}");
    Console.WriteLine($"  {link.DownloadUrl}");
}
```

`AddAsync` always returns a list. A normal link gives one entry, a folder link gives one entry per file.

```csharp
// The daily limits. By default the answer has the hosts that the account uses right now, with all: true every host.
DownloaderLimits limits = await client.Downloader.GetLimitsAsync(all: true);

Console.WriteLine($"{limits.UsagePercent:0.##} % used, next reset in {limits.NextReset.TotalHours:0.#} hours");

// The limits of a single host.
HosterLimit? rapidgator = limits.Get(Hoster.RapidGator);

// The hosts that Debrid-Link supports.
IReadOnlyList<DownloaderHost> hosts = await client.Downloader.GetHostsAsync();

// Remove links by id. The answer has the ids that were removed.
IReadOnlyList<string> removed = await client.Downloader.RemoveAsync(linkId);
```

One client can be reused for many requests and from several threads.

### Endpoints

| Area         | Method                                                                      | API endpoint                                                      |
|--------------|-----------------------------------------------------------------------------|-------------------------------------------------------------------|
| `Account`    | `GetInfoAsync(cancellationToken)`                                           | `GET /account/infos`                                              |
| `Downloader` | `AddAsync(url, password?, ip?, cancellationToken)`                          | `POST /downloader/add`                                            |
| `Downloader` | `GetLimitsAsync(all?, cancellationToken)`                                   | `GET /downloader/limits`, with `all` `GET /downloader/limits/all` |
| `Downloader` | `GetHostsAsync(types?, cancellationToken)`                                  | `GET /downloader/hosts`                                           |
| `Downloader` | `RemoveAsync(id, cancellationToken)`, `RemoveAsync(ids, cancellationToken)` | `DELETE /downloader/{ids}/remove`                                 |

More endpoints will follow, grouped by the areas of the API documentation (the rest of `Downloader`, `Seedbox`, `Files`, ...).

### The data model

| Type               | Members                                                                                                                                                                               |
|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `AccountInfo`      | `Username`, `Email`, `EmailVerified`, `AccountType`, `PremiumLeft`, `Points`, `RegisterDate`, `ServerDetected`, `Settings`, `AvatarUrl` and `UpgradeAccountUrl`                       |
| `AccountSettings`  | `Https`, `ThemeDark`, `HideOldLinks`, `Cdn`, `TwoFactorType`, `EmailsNews`, `EmailsAccount`, `EmailsSupport`, `IpVersion`                                                             |
| `AccountType`      | `Free`, `Premium`, `LifetimePremium`                                                                                                                                                  |
| `Cdn`              | `Unknown`, `Auto`, `Direct`, `Cloudflare`, `Scaleway`                                                                                                                                 |
| `IpVersion`        | `Unknown`, `Auto`, `IPv4`                                                                                                                                                             |
| `TwoFactorType`    | `Unknown`, `Email`, `Otp`                                                                                                                                                             |
| `DownloaderLink`   | `Id`, `Name`, `Url`, `DownloadUrl`, `Host`, `Size`, `Chunk`, `Created`, `Expired`, `IsProcessing`                                                                                     |
| `DownloaderHost`   | `Name`, `Hoster`, `Type`, `Status`, `IsFree`, `IsOnline`, `Domains`, `Regexs`                                                                                                         |
| `HostType`         | `Unknown`, `Host`, `Stream`, `Protect`                                                                                                                                                |
| `DownloaderLimits` | `UsagePercent`, `NextReset`, `Hosters`                                                                                                                                                |
| `HosterLimit`      | `Hoster`, `DaySize`, `DayCount`                                                                                                                                                       |
| `Limit`            | `Current`, `Value`                                                                                                                                                                    |
| `Hoster`           | `Unknown`, `Other`, then a member for every file host (`RapidGator`, `Mega`, `OneFichier`, ...) and, at the end, for every streaming site (`Dailymotion`, `Soundcloud`, `Vimeo`, ...) |

The account types are in the namespace `DebridLinkSharp.Models.Account` and the downloader types in `DebridLinkSharp.Models.Downloader`.

`Size` can be an estimation of the final size. If `IsProcessing` is `true`, the file is still being prepared on the servers of Debrid-Link
and it is better to wait before the download is started. `Created` is a `DateTimeOffset`. A link with `Expired` set has to be added again with `AddAsync`.

`PremiumLeft` is a `TimeSpan` and always zero for a free account. `RegisterDate` is a `DateOnly`. `Username`, `Email`, `Settings`, `AvatarUrl` and `UpgradeAccountUrl` are always set. `Email` is already masked by Debrid-Link (`p**d@deb*******k`). `UpgradeAccountUrl` and `AvatarUrl` are links specific to your account, so do not log or share them.

`Cdn`, `IpVersion` and `TwoFactorType` of `AccountSettings` are enums with an `Unknown` member (the first one, so it is also the default). It is used when Debrid-Link sends no value or a value that this version does not know yet, so a new option there does not make the call fail.
`GetHostsAsync` lists the hosts that Debrid-Link supports. A host is online when its `Status` is 1 or higher (`IsOnline`). `Type` is `Host` for file hosts and `Stream` for streaming sites, `Protect` has no entries at the moment. With a key the list has 167 hosts (54 file hosts and 113 streaming sites) at the time of writing. Without a key it has only 43 file hosts, and an invalid key gives `badToken`. Without `types`, the answer has every host. Pass some `HostType`s to ask for some of them, for example `[HostType.Host]` for the file hosts. `Name` is the name of the host as Debrid-Link sends it. `Hoster` is the same host as an enum, and `Unknown` if the enum does not have it, which is the case for a host that Debrid-Link added after the library was written.

`RemoveAsync` takes one id or several and answers with the ids that it was asked to remove. The API does not say whether a link with that id existed: an id with the right form that belongs to no link comes back as well. An id with a wrong form gives `badIds` (the table of the documentation has `badId`).

`GetLimitsAsync` has the hosts that the account uses right now. With `all: true` it asks for the limits of all hosts (`/downloader/limits/all`). `UsagePercent` is a number from 0 to 100, `NextReset` is the time until the daily limits are reset (in most cases until the next day). The API sends both as a pair of numbers, `current` and `value`, and the library takes the one that matters. For every host, a `Limit` has `Current` (what is used today) and `Value` (the limit); `DaySize` is in bytes and `DayCount` in links.

`Hoster` has a member for every host that Debrid-Link lists for a key when the library was written: 54 file hosts, then 113 streaming sites, 167 in all (the same split as the `Type` of a `DownloaderHost`). A host that is not in it is read as `Unknown`, so a new host does not make a call fail, but its name is not kept: the enum has to be extended then. The API sends the limits only for some hosts and one entry for all the others together, which the library calls `Hoster.Other`. `DownloaderLimits.Get(hoster)` gives the limits of one host out of an answer: the entry of the host, or the `Other` entry if the host has none of its own, or `null` if the answer has neither. Use it with `GetLimitsAsync(all: true)`, because the default answer only has the hosts that are in use.

## Options

```csharp
var options = new DebridLinkClientOptions
{
    BaseUrl = new Uri("https://my-proxy.example/api/v2/"),  // default: https://debrid-link.com/api/v2/
    Timeout = TimeSpan.FromSeconds(10),                     // default 30 seconds
};

// Optionally bring your own HttpClient (proxy, logging, IHttpClientFactory). It is not disposed.
var client = new DebridLinkClient(new HttpClient(), apiKey, options);
```

Without an `HttpClient` a shared static client is used, so creating many clients does not leak sockets.
The options are validated and copied when the client is created.

## Errors

All exceptions derive from `DebridLinkException`. That class is `closed`: only the two exceptions below derive from it, so the compiler checks that a `switch` over a `DebridLinkException` covers both. It cannot be derived from outside the library.

| Exception                    | When                                                                                                                                                                              |
|------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DebridLinkApiException`     | The API answered with an error. `ErrorCode` is the code of the API, for example `badToken`, `hostNotValid` or `maxLink` (see `DebridLinkErrors`). `StatusCode` is the HTTP status |
| `DebridLinkServiceException` | The answer could not be used. Check `Error`: `Unreachable`, `Timeout` or `InvalidResponse` (not JSON, or not shaped like the documentation)                                       |
| `OperationCanceledException` | Your `CancellationToken` was cancelled                                                                                                                                            |

Invalid arguments are rejected before any request is sent.

## Examples

A short, runnable example lives in [examples/DebridLinkSharp.Examples](https://github.com/MCmoderSD/Debrid-Link-Sharp/tree/master/examples/DebridLinkSharp.Examples). Run it from a clone of the repository.
The API key is read from the environment variable `DEBRID_LINK_API_KEY`, so it does not end up in your shell history:

```bash
export DEBRID_LINK_API_KEY=your-key
dotnet run --project examples/DebridLinkSharp.Examples -- add https://hoster.example/file/abc
dotnet run --project examples/DebridLinkSharp.Examples -- account
dotnet run --project examples/DebridLinkSharp.Examples -- limits
dotnet run --project examples/DebridLinkSharp.Examples -- limits-all
dotnet run --project examples/DebridLinkSharp.Examples -- hosts
dotnet run --project examples/DebridLinkSharp.Examples -- remove <id[,id...]>
```

| Example      | What it shows                                                                                      |
|--------------|----------------------------------------------------------------------------------------------------|
| `add`        | Add a hoster link, print the direct download URL and handle the errors of the API                  |
| `account`    | Show everything Debrid-Link tells about the account, in sections, and handle the errors of the API |
| `limits`     | Show the daily limits and how much of them is used (the hosts that the account uses right now)     |
| `limits-all` | Show the daily limits of every host                                                                |
| `hosts`      | List the hosts that Debrid-Link supports, with their type and status                               |
| `remove`     | Remove links from the account by id, separated by commas                                           |

## Known limits

- **Early version:** Only `GET /account/infos`, `POST /downloader/add`, `GET /downloader/limits`, `GET /downloader/hosts` and `DELETE /downloader/{ids}/remove` are implemented so far. The other endpoints (the rest of the downloader, seedbox, ...) and OAuth2 helpers (device code, refresh token) follow.
- **Network and privacy:** Every call sends your API key, and the link you add, to `debrid-link.com` over HTTPS. `BaseUrl` can be pointed at your own proxy.
- **OAuth2 scopes:** With an OAuth2 access token instead of a private API key, the token needs the scope `get.account` for the account, `post.downloader` for adding links, `get.downloader` for the limits and the hosts, and `delete.downloader` for removing links.
- **Personal use:** The documentation of Debrid-Link says that the API is not meant to resell their services and that an account is for personal use.
- **No rate limiting:** The library does not slow down or repeat requests. If you send too many requests, the API answers with `floodDetected` and blocks the account for about an hour, so space your requests yourself.
- **Folders:** A folder link returns several entries. Each entry has its own `DownloadUrl`.

## Tests

The test project `tests/DebridLinkSharp.Tests` is set up like the one of DLC-Sharp (xUnit v3 on .NET 11, coverage with `Microsoft.Testing.Extensions.CodeCoverage`). The tests themselves follow in a later release.

## License

BSD 3-Clause, see [LICENSE](https://github.com/MCmoderSD/Debrid-Link-Sharp/blob/master/LICENSE). "Debrid-Link" is a name of its owner and is used here only to describe compatibility.
