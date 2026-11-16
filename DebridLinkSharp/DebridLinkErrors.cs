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

    public const string BadIds = "badIds";

    public const string InfringingFile = "infringingFile";

    
    // Downloader

    public const string NotDebrid = "notDebrid";

    public const string HostNotValid = "hostNotValid";

    public const string FileNotFound = "fileNotFound";

    public const string FileNotAvailable = "fileNotAvailable";

    public const string BadFileUrl = "badFileUrl";

    public const string BadFilePassword = "badFilePassword";

    public const string NotFreeHost = "notFreeHost";

    public const string MaintenanceHost = "maintenanceHost";

    public const string MaxLink = "maxLink";

    public const string MaxLinkHost = "maxLinkHost";

    public const string MaxData = "maxData";

    public const string MaxDataHost = "maxDataHost";

    
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
            BadId or BadIds => "The provided ID(s) are invalid.",
            InfringingFile => "The file is unavailable for legal reasons.",
            NotDebrid => "Unable to generate a download link for this link, please retry later.",
            HostNotValid => "This host or domain is not supported.",
            FileNotFound => "The file was not found on the host.",
            FileNotAvailable => "The file is temporarily unavailable on the host.",
            BadFileUrl => "The link format is invalid.",
            BadFilePassword => "The file password is incorrect.",
            NotFreeHost => "This host is not available for free users.",
            MaintenanceHost => "This host is under maintenance, please retry later.",
            MaxLink => "The daily link limit is reached.",
            MaxLinkHost => "The daily link limit for this host is reached.",
            MaxData => "The daily data limit is reached.",
            MaxDataHost => "The daily data limit for this host is reached.",
            _ => null
        };
    }
}