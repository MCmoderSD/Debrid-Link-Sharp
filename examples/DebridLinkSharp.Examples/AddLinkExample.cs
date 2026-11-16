using DebridLinkSharp;

namespace Examples;

// The simplest use: add a hoster link and print what Debrid-Link returns.
// Two kinds of errors can happen: the API refuses the link, or it cannot be used at all.
// Both derive from DebridLinkException, so a single catch (DebridLinkException) handles them. ErrorAdvice turns them into a line of advice.
internal static class AddLinkExample
{
    public static async Task RunAsync(string apiKey, string url)
    {
        // One client can be created once and reused for many requests, also from several threads.
        var client = new DebridLinkClient(apiKey);

        try
        {
            // The result is always a list: one entry for a normal link, several for a folder.
            var links = await client.Downloader.AddAsync(url);

            Console.WriteLine($"Debrid-Link returned {links.Count} link(s).");

            foreach (var link in links)
            {
                Console.WriteLine();
                Console.WriteLine($"Name:    {link.Name}");
                Console.WriteLine($"Host:    {link.Host}");

                // The size can be an estimation of the final size.
                Console.WriteLine($"Size:    {link.Size} bytes");
                Console.WriteLine($"Created: {link.Created:u}");
                Console.WriteLine($"Download: {link.DownloadUrl}");

                if (link.IsProcessing)
                {
                    Console.WriteLine("The file is still being prepared on the servers of Debrid-Link. Wait before the download is started.");
                }
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
