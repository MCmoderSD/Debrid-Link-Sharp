using DebridLinkSharp;

namespace Examples;

// Removes links from the account. The ids are separated by commas.
internal static class RemoveLinksExample
{
    public static async Task RunAsync(string apiKey, string idList)
    {
        var ids = idList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (ids.Length == 0)
        {
            Console.WriteLine("Pass at least one link id, several ones separated by commas.");
            return;
        }

        var client = new DebridLinkClient(apiKey);

        try
        {
            // The answer has the ids of the links that were removed.
            var removed = await client.Downloader.RemoveAsync(ids);

            Console.WriteLine($"Removed {removed.Count} of {ids.Length} link(s):");
            foreach (var id in removed)
            {
                Console.WriteLine($"  {id}");
            }
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }
}
