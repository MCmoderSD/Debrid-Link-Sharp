using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderLimits
{
    // How much of the daily limit is used, from 0 to 100.
    [JsonConverter(typeof(UsagePercentConverter))]
    public double UsagePercent { get; init; }

    // The time until the daily limits are reset, in most cases until the next day.
    [JsonPropertyName("nextResetSeconds")]
    [JsonConverter(typeof(ResetPeriodConverter))]
    public TimeSpan NextReset { get; init; }

    // The hosts that the account uses right now, or all hosts if GetLimitsAsync was called with all.
    public IReadOnlyList<HosterLimit> Hosters { get; init; } = [];

    // The limits of one host. A host without an entry of its own is covered by the Other entry, which the API sends for all of them together.
    // The result is null if the answer has neither. Unknown never matches, because it can be any host that is not known yet.
    // Ask for GetLimitsAsync(all: true) to have the limits of every host. The default answer only has the hosts that are in use.
    public HosterLimit? Get(Hoster hoster)
    {
        if (hoster == Hoster.Unknown)
        {
            return null;
        }

        var own = Hosters.FirstOrDefault(limit => limit.Hoster == hoster);
        return own ?? (hoster == Hoster.Other ? null : Hosters.FirstOrDefault(limit => limit.Hoster == Hoster.Other));
    }
}
