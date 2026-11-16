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
        DebridLinkErrors.UnknownEndpoint => "The API does not know this endpoint. The library may be out of date.",
        DebridLinkErrors.InternalError => "Debrid-Link had an internal error. Try again later.",
        DebridLinkErrors.BadToken => "The API key is invalid or expired. Create a new one.",
        DebridLinkErrors.BadArguments => "A parameter is missing or invalid. Check what you passed.",
        DebridLinkErrors.FloodDetected => "Too many requests. The account is blocked for about an hour.",
        DebridLinkErrors.FreeServerOverload => "The free servers are overloaded. Try again later, or upgrade to premium.",
        DebridLinkErrors.UnverifiedEmail => "Verify the e-mail address of the account first, or upgrade to premium.",
        DebridLinkErrors.BadId or DebridLinkErrors.BadIds => "An id is not valid. Use the ids of the links of the account.",
        DebridLinkErrors.InfringingFile => "The file is unavailable for legal reasons.",
        DebridLinkErrors.NotDebrid => "Debrid-Link could not generate a download link. Try again later.",
        DebridLinkErrors.HostNotValid => "Debrid-Link does not support this host.",
        DebridLinkErrors.FileNotFound => "The file is gone from the host.",
        DebridLinkErrors.FileNotAvailable => "The file is not available on the host right now. Try again later.",
        DebridLinkErrors.BadFileUrl => "The link has an invalid format. Check the URL.",
        DebridLinkErrors.BadFilePassword => "The host protects this file with a password.",
        DebridLinkErrors.NotFreeHost => "This host is not available for free accounts. Upgrade to premium.",
        DebridLinkErrors.MaintenanceHost => "The host is under maintenance. Try again later.",
        DebridLinkErrors.MaxLink or DebridLinkErrors.MaxLinkHost => "The daily link limit is reached.",
        DebridLinkErrors.MaxData or DebridLinkErrors.MaxDataHost => "The daily data limit is reached.",
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
