namespace DebridLinkSharp;

public static class DebridLinkErrors
{
    // All services

    public const string UnknownEndpoint = "unknowR";

    public const string InternalError = "internalError";

    public const string BadToken = "badToken";

    public const string BadArguments = "badArguments";

    public const string FloodDetected = "floodDetected";

    public const string FreeServerOverload = "freeServerOverload";

    public const string UnverifiedEmail = "unverifiedEmail";

    public const string BadId = "badId";

    public const string InfringingFile = "infringingFile";

    internal static string? Describe(string errorCode)
    {
        return errorCode switch
        {
            UnknownEndpoint => "Unknown API endpoint.",
            InternalError => "An internal error occurred, please retry later.",
            BadToken => "The API key or access token is invalid or expired.",
            BadArguments => "One or more parameters are missing or invalid.",
            FloodDetected => "Too many requests, the account is blocked for about an hour.",
            FreeServerOverload => "The free servers are overloaded, please retry later.",
            UnverifiedEmail => "A free account has to verify its email address first.",
            BadId => "The provided ID(s) are invalid.",
            InfringingFile => "The file is unavailable for legal reasons.",
            _ => null
        };
    }
}