namespace Examples;

internal static class Program
{
    private const string ApiKeyVariable = "DEBRID_LINK_API_KEY";

    private sealed record Example(string Usage, string Description, bool NeedsArgument, Func<string, string?, Task> Run);

    // Every example gets the API key, which is read from the environment so it stays out of the shell history, and, if it needs one, an argument.
    private static readonly Dictionary<string, Example> Examples = new()
    {
        ["limits"] = new Example("limits", "Show the daily limits and how much of them is used", false, (apiKey, _) => LimitsExample.RunAsync(apiKey)),
        ["limits-all"] = new Example("limits-all", "Show the daily limits of every host", false, (apiKey, _) => LimitsExample.RunAsync(apiKey, all: true)),
        ["hosts"] = new Example("hosts", "List the hosts that Debrid-Link supports", false, (apiKey, _) => HostsExample.RunAsync(apiKey)),
        ["remove"] = new Example("remove <id[,id...]>", "Remove links from the account by id", true, (apiKey, ids) => RemoveLinksExample.RunAsync(apiKey, ids!)),
        ["account"] = new Example("account", "Show everything Debrid-Link tells about the account, in sections", false, (apiKey, _) => AccountInfoExample.RunAsync(apiKey))
    };

    private static async Task<int> Main(string[] args)
    {
        if (args.Length < 1 || !Examples.TryGetValue(args[0], out var example) || (example.NeedsArgument && args.Length < 2))
        {
            PrintUsage();
            return 1;
        }

        var apiKey = Environment.GetEnvironmentVariable(ApiKeyVariable);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine($"Set the environment variable {ApiKeyVariable} to your Debrid-Link API key (https://debrid-link.com/webapp/apikey).");
            return 1;
        }

        await example.Run(apiKey, args.Length > 1 ? args[1] : null);
        return 0;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run --project examples/DebridLinkSharp.Examples -- <example> [argument]");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        foreach (var example in Examples.Values)
        {
            Console.WriteLine($"  {example.Usage,-22}{example.Description}");
        }

        Console.WriteLine();
        Console.WriteLine($"The API key is read from the environment variable {ApiKeyVariable}.");
        Console.WriteLine("Note: the key is sent to debrid-link.com.");
    }
}
