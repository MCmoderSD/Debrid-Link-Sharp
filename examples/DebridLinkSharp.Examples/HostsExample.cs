using DebridLinkSharp;

namespace Examples;

// Lists the hosts that Debrid-Link can generate download links for.
internal static class HostsExample
{
    public static async Task RunAsync(string apiKey)
    {
        var client = new DebridLinkClient(apiKey);

        try
        {
            var hosts = await client.Downloader.GetHostsAsync();

            Console.WriteLine($"{hosts.Count} hosts, {hosts.Count(host => host.IsOnline)} online, {hosts.Count(host => host.IsFree)} free");
            Console.WriteLine();

            foreach (var host in hosts.OrderBy(host => host.Hoster.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                var domains = string.Join(", ", host.Domains.Take(3)) + (host.Domains.Count > 3 ? ", ..." : "");
                Console.WriteLine($"  {host.Hoster,-20}{(host.IsOnline ? "online " : "offline")}  {(host.IsFree ? "free   " : "premium")}  {domains}");
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
