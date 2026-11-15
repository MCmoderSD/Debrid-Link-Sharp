using DebridLinkSharp;
using DebridLinkSharp.Models;
using DebridLinkSharp.Models.Account;

namespace Examples;

// Shows who the API key belongs to and everything Debrid-Link tells about the account, in sections.
// An OAuth2 access token needs the scope get.account for this.
internal static class AccountInfoExample
{
    public static async Task RunAsync(string apiKey)
    {
        var client = new DebridLinkClient(apiKey);

        try
        {
            AccountInfo account = await client.Account.GetInfoAsync();

            // PremiumLeft is always zero for a free account. For a lifetime account there is no end to wait for.
            var premium = account.AccountType switch
            {
                AccountType.Free => "none",
                AccountType.LifetimePremium => "lifetime, no end",
                _ => $"{account.PremiumLeft.TotalDays:0.#} days left, until {DateTimeOffset.UtcNow.Add(account.PremiumLeft):yyyy-MM-dd}"
            };

            Section("Account");
            Row("User", account.Username);
            Row("E-mail", $"{account.Email} ({(account.EmailVerified ? "verified" : "not verified")})");
            Row("Registered", $"{account.RegisterDate:yyyy-MM-dd}");
            Row("Avatar", account.AvatarUrl);

            Section("Subscription");
            Row("Type", account.AccountType);
            Row("Premium", premium);
            Row("Points", account.Points);
            Row("Upgrade page", account.UpgradeAccountUrl);

            var settings = account.Settings;

            Section("Settings");
            Row("HTTPS", YesNo(settings.Https));
            Row("Dark theme", YesNo(settings.ThemeDark));
            Row("Hide old links", YesNo(settings.HideOldLinks));
            Row("CDN", settings.Cdn);
            Row("IP version", settings.IpVersion);
            Row("Two-factor", settings.TwoFactorType);

            Section("E-mail notifications");
            Row("News", YesNo(settings.EmailsNews));
            Row("Account", YesNo(settings.EmailsAccount));
            Row("Support", YesNo(settings.EmailsSupport));

            Section("Connection");
            Row("Server detected", YesNo(account.ServerDetected));
        }
        catch (DebridLinkException ex)
        {
            Console.WriteLine(ErrorAdvice.Describe(ex));
        }
    }

    // A blank line, the title and a line below it.
    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine(new string('-', title.Length));
    }

    private static void Row(string label, object value) => Console.WriteLine($"  {label,-16}{value}");

    private static string YesNo(bool value) => value ? "yes" : "no";
}
