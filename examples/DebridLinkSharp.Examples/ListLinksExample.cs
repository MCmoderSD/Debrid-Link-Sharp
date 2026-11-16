using DebridLinkSharp;
using DebridLinkSharp.Models.Downloader;

namespace Examples;

// Lists the links that were added to the account. The list comes in pages, this example shows the first one.
internal static class ListLinksExample
{
    public static async Task RunAsync(string apiKey)
    {
        var client = new DebridLinkClient(apiKey);

        try
        {
            var page = await client.Downloader.ListAsync();

            Console.WriteLine($"Page {page.Page + 1} of {Math.Max(page.Pages, 1)}, {page.Items.Count} link(s)");

            foreach (DownloaderLink link in page.Items)
            {
                var status = link.Expired ? "expired, add it again" : link.IsProcessing ? "still processing" : "ready";

                Console.WriteLine();
                Console.WriteLine($"  {link.Name}");
                Console.WriteLine($"    Id      {link.Id}");
                Console.WriteLine($"    Host    {link.Host}");
                Console.WriteLine($"    Size    {Format.Size(link.Size)}");
                Console.WriteLine($"    Added   {link.Created:u}");
                Console.WriteLine($"    Status  {status}");
            }

            if (page.HasNext)
            {
                Console.WriteLine();
                Console.WriteLine($"There is more: call ListAsync(page: {page.Next}) for the next page.");
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
