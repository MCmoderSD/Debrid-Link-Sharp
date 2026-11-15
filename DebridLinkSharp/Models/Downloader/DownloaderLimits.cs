using System.Text.Json.Serialization;
using DebridLinkSharp.Internal;

namespace DebridLinkSharp.Models.Downloader;

public sealed class DownloaderLimits
{
    [JsonConverter(typeof(UsagePercentConverter))]
    public double UsagePercent { get; init; }

    [JsonPropertyName("nextResetSeconds")]
    [JsonConverter(typeof(ResetPeriodConverter))]
    public TimeSpan NextReset { get; init; }

    public IReadOnlyList<HosterLimit> Hosters { get; init; } = [];

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