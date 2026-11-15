# DebridLinkSharp

A small .NET client for the **[Debrid-Link](https://debrid-link.com) API v2**. For now it reads your account: the account type, the premium time left, the points and the settings, as plain objects. It also reads the daily limits of the downloader.

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
// The daily limits. By default the answer has the hosts that the account uses right now, with all: true every host.
DownloaderLimits limits = await client.Downloader.GetLimitsAsync(all: true);

Console.WriteLine($"{limits.UsagePercent:0.##} % used, next reset in {limits.NextReset.TotalHours:0.#} hours");

// The limits of a single host.
HosterLimit? rapidgator = limits.Get(Hoster.RapidGator);

// The hosts that Debrid-Link supports.
IReadOnlyList<DownloaderHost> hosts = await client.Downloader.GetHostsAsync();
```

One client can be reused for many requests and from several threads.

### Endpoints

| Area         | Method                                     | API endpoint                                                      |
|--------------|--------------------------------------------|-------------------------------------------------------------------|
| `Account`    | `GetInfoAsync(cancellationToken)`          | `GET /account/infos`                                              |
| `Downloader` | `GetLimitsAsync(all?, cancellationToken)`  | `GET /downloader/limits`, with `all` `GET /downloader/limits/all` |
| `Downloader` | `GetHostsAsync(types?, cancellationToken)` | `GET /downloader/hosts`                                           |

More endpoints will follow, grouped by the areas of the API documentation (the rest of `Downloader`, `Seedbox`, `Files`, ...).

### The data model

| Type               | Members                                                                                                                                                         |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `AccountInfo`      | `Username`, `Email`, `EmailVerified`, `AccountType`, `PremiumLeft`, `Points`, `RegisterDate`, `ServerDetected`, `Settings`, `AvatarUrl` and `UpgradeAccountUrl` |
| `AccountSettings`  | `Https`, `ThemeDark`, `HideOldLinks`, `Cdn`, `TwoFactorType`, `EmailsNews`, `EmailsAccount`, `EmailsSupport`, `IpVersion`                                       |
| `AccountType`      | `Free`, `Premium`, `LifetimePremium`                                                                                                                            |
| `Cdn`              | `Unknown`, `Auto`, `Direct`, `Cloudflare`, `Scaleway`                                                                                                           |
| `IpVersion`        | `Unknown`, `Auto`, `IPv4`                                                                                                                                       |
| `TwoFactorType`    | `Unknown`, `Email`, `Otp`                                                                                                                                       |
| `DownloaderHost`   | `Name`, `Hoster`, `Type`, `Status`, `IsFree`, `IsOnline`, `Domains`, `Regexs`                                                                                   |
| `HostType`         | `Unknown`, `Host`, `Stream`, `Protect`                                                                                                                          |
| `DownloaderLimits` | `UsagePercent`, `NextReset`, `Hosters`                                                                                                                          |
| `HosterLimit`      | `Hoster`, `DaySize`, `DayCount`                                                                                                                                 |
| `Limit`            | `Current`, `Value`                                                                                                                                              |
| `Hoster`           | `Unknown`, `Other` and one member for every host, for example `RapidGator`, `Mega`, `MediaFire`, `OneFichier`                                                   |

`PremiumLeft` is a `TimeSpan` and always zero for a free account. `RegisterDate` is a `DateOnly`. `Username`, `Email`, `Settings`, `AvatarUrl` and `UpgradeAccountUrl` are always set. `Email` is already masked by Debrid-Link (`p**d@deb*******k`). `UpgradeAccountUrl` and `AvatarUrl` are links specific to your account, so do not log or share them.

`Cdn`, `IpVersion` and `TwoFactorType` of `AccountSettings` are enums with an `Unknown` member (the first one, so it is also the default). It is used when Debrid-Link sends no value or a value that this version does not know yet, so a new option there does not make the call fail.
`GetHostsAsync` lists the hosts that Debrid-Link supports. A host is online when its `Status` is 1 or higher (`IsOnline`). `Type` is `Host` for file hosts and `Stream` for streaming sites, `Protect` has no entries at the moment. With a key the list has 167 hosts (54 file hosts and 113 streaming sites) at the time of writing. Without a key it has only 43 file hosts, and an invalid key gives `badToken`. Without `types`, the answer has every host. Pass some `HostType`s to ask for some of them, for example `[HostType.Host]` for the file hosts. `Name` is the name of the host as Debrid-Link sends it. `Hoster` is the same host as an enum, and `Unknown` if the enum does not have it, which is the case for every streaming site and for some file hosts.

`GetLimitsAsync` has the hosts that the account uses right now. With `all: true` it asks for the limits of all hosts (`/downloader/limits/all`). `UsagePercent` is a number from 0 to 100, `NextReset` is the time until the daily limits are reset (in most cases until the next day). The API sends both as a pair of numbers, `current` and `value`, and the library takes the one that matters. For every host, a `Limit` has `Current` (what is used today) and `Value` (the limit); `DaySize` is in bytes and `DayCount` in links.

`Hoster` has a member for every host that Debrid-Link supported when the library was written (43). A host that is not in it is read as `Unknown`, so a new host does not make a call fail, but its name is not kept: the enum has to be extended then. The API sends the limits only for some hosts and one entry for all the others together, which the library calls `Hoster.Other`. `DownloaderLimits.Get(hoster)` gives the limits of one host out of an answer: the entry of the host, or the `Other` entry if the host has none of its own, or `null` if the answer has neither. Use it with `GetLimitsAsync(all: true)`, because the default answer only has the hosts that are in use.

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

| Exception                    | When                                                                                                                                                                    |
|------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DebridLinkApiException`     | The API answered with an error. `ErrorCode` is the code of the API, for example `badToken` or `floodDetected` (see `DebridLinkErrors`). `StatusCode` is the HTTP status |
| `DebridLinkServiceException` | The answer could not be used. Check `Error`: `Unreachable`, `Timeout` or `InvalidResponse` (not JSON, or not shaped like the documentation)                             |
| `OperationCanceledException` | Your `CancellationToken` was cancelled                                                                                                                                  |

Invalid arguments are rejected before any request is sent.

## Examples

A short, runnable example lives in [examples/DebridLinkSharp.Examples](https://github.com/MCmoderSD/Debrid-Link-Sharp/tree/master/examples/DebridLinkSharp.Examples). Run it from a clone of the repository.
The API key is read from the environment variable `DEBRID_LINK_API_KEY`, so it does not end up in your shell history:

```bash
export DEBRID_LINK_API_KEY=your-key
dotnet run --project examples/DebridLinkSharp.Examples -- account
dotnet run --project examples/DebridLinkSharp.Examples -- limits
dotnet run --project examples/DebridLinkSharp.Examples -- limits-all
dotnet run --project examples/DebridLinkSharp.Examples -- hosts
```

| Example      | What it shows                                                                                      |
|--------------|----------------------------------------------------------------------------------------------------|
| `account`    | Show everything Debrid-Link tells about the account, in sections, and handle the errors of the API |
| `limits`     | Show the daily limits and how much of them is used (the hosts that the account uses right now)     |
| `limits-all` | Show the daily limits of every host                                                                |
| `hosts`      | List the hosts that Debrid-Link supports, with their type and status                               |

## Known limits

- **Early version:** Only `GET /account/infos`, `GET /downloader/limits` and `GET /downloader/hosts` are implemented so far. The other endpoints (the rest of the downloader, seedbox, ...) and OAuth2 helpers (device code, refresh token) follow.
- **Network and privacy:** Every call sends your API key to `debrid-link.com` over HTTPS. `BaseUrl` can be pointed at your own proxy.
- **OAuth2 scopes:** With an OAuth2 access token instead of a private API key, the token needs the scope `get.account` for the account and `get.downloader` for the limits.
- **Personal use:** The documentation of Debrid-Link says that the API is not meant to resell their services and that an account is for personal use.
- **No rate limiting:** The library does not slow down or repeat requests. If you send too many requests, the API answers with `floodDetected` and blocks the account for about an hour, so space your requests yourself.

## Tests

The test project `tests/DebridLinkSharp.Tests` is set up like the one of DLC-Sharp (xUnit v3 on .NET 11, coverage with `Microsoft.Testing.Extensions.CodeCoverage`). The tests themselves follow in a later release.

## License

BSD 3-Clause, see [LICENSE](https://github.com/MCmoderSD/Debrid-Link-Sharp/blob/master/LICENSE). "Debrid-Link" is a name of its owner and is used here only to describe compatibility.
