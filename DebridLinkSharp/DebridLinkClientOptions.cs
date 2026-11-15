namespace DebridLinkSharp;

public sealed class DebridLinkClientOptions
{
    private const string DefaultBaseUrl = "https://debrid-link.com/api/v2/";

    public Uri BaseUrl { get; set; } = new(DefaultBaseUrl);

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    internal DebridLinkClientOptions CloneAndValidate()
    {
        if (BaseUrl == null || !BaseUrl.IsAbsoluteUri || (BaseUrl.Scheme != Uri.UriSchemeHttp && BaseUrl.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("BaseUrl must be an absolute http or https URL.", nameof(BaseUrl));
        }

        if (Timeout <= TimeSpan.Zero && Timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentException("Timeout must be positive or Timeout.InfiniteTimeSpan.", nameof(Timeout));
        }

        var baseUrl = BaseUrl.AbsoluteUri;
        if (!baseUrl.EndsWith('/'))
        {
            baseUrl += "/";
        }

        return new DebridLinkClientOptions
        {
            BaseUrl = new Uri(baseUrl),
            Timeout = Timeout
        };
    }
}