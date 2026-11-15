using DebridLinkSharp;

namespace Examples;

// Turns the exceptions of the library into one line of advice.
// DebridLinkException is closed: the compiler checks that the switch covers both kinds, so it needs no default arm.
internal static class ErrorAdvice
{
    public static string Describe(DebridLinkException ex) => ex switch
    {
        DebridLinkApiException api => $"Debrid-Link refused the request ({api.ErrorCode}): {Advice(api)}",
        DebridLinkServiceException service => $"No usable answer: {Advice(service)}"
    };

    // The API answered with an error. ErrorCode tells which one, DebridLinkErrors lists the known codes.
    private static string Advice(DebridLinkApiException ex) => ex.ErrorCode switch
    {
        DebridLinkErrors.BadToken => "The API key is invalid or expired. Create a new one.",
        DebridLinkErrors.FloodDetected => "Too many requests. The account is blocked for about an hour.",
        DebridLinkErrors.UnverifiedEmail => "Verify the e-mail address of the account first, or upgrade to premium.",
        _ => ex.Message
    };

    // There was no usable answer. Error tells why.
    private static string Advice(DebridLinkServiceException ex) => ex.Error switch
    {
        DebridLinkServiceError.Unreachable => "No connection. Check the network.",
        DebridLinkServiceError.Timeout => "Debrid-Link was too slow. Raise DebridLinkClientOptions.Timeout or try again later.",
        DebridLinkServiceError.InvalidResponse => $"The answer could not be read (HTTP {(int?)ex.StatusCode}).",
        _ => "Unknown problem."
    };
}
