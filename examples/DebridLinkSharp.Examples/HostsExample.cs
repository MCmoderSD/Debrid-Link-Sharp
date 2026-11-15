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
            // Without types, the answer has every host. Pass some HostTypes to ask for some of them, for example GetHostsAsync([HostType.Host]) for the file hosts.
            var hosts = await client.Downloader.GetHostsAsync();

            Console.WriteLine($"{hosts.Count} hosts, {hosts.Count(host => host.IsOnline)} online, {hosts.Count(host => host.IsFree)} free");
            Console.WriteLine($"Types: {string.Join(", ", hosts.GroupBy(host => host.Type).Select(group => $"{group.Key} {group.Count()}"))}");
            Console.WriteLine();

            foreach (var host in hosts.OrderBy(host => host.Type).ThenBy(host => host.Name, StringComparer.OrdinalIgnoreCase))
            {
                var domains = string.Join(", ", host.Domains.Take(3)) + (host.Domains.Count > 3 ? ", ..." : "");
                Console.WriteLine($"  {host.Name,-26}{host.Type,-8}{(host.IsOnline ? "online " : "offline")}  {(host.IsFree ? "free   " : "premium")}  {domains}");
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
