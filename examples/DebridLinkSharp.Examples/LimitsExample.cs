using DebridLinkSharp;

namespace Examples;

// Shows the daily limits of the downloader and how much of them is used.
// By default the answer has the hosts that the account uses right now. With all, it has every host.
internal static class LimitsExample
{
    public static async Task RunAsync(string apiKey, bool all = false)
    {
        var client = new DebridLinkClient(apiKey);

        try
        {
            var limits = await client.Downloader.GetLimitsAsync(all);

            Console.WriteLine($"Usage        {limits.UsagePercent:0.##} %");
            Console.WriteLine($"Next reset   in {(int)limits.NextReset.TotalHours} h {limits.NextReset.Minutes} min");

            Console.WriteLine();
            if (limits.Hosters.Count == 0)
            {
                Console.WriteLine(all ? "The answer has no hosts." : "No host is used right now. Run limits-all to see every host.");
                return;
            }

            Console.WriteLine(all ? $"All hosts ({limits.Hosters.Count})" : $"Hosts in use ({limits.Hosters.Count})");
            foreach (var hoster in limits.Hosters)
            {
                // Every limit of a host is a pair: Current is what is used today, Value is the limit.
                // The entry Other is the limit that all hosts share which have no entry of their own.
                Console.WriteLine($"  {hoster.Hoster,-20}{Format.Size(hoster.DaySize.Current)} of {Format.Size(hoster.DaySize.Value)}, {hoster.DayCount.Current} of {hoster.DayCount.Value} links");
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
