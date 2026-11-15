using System.Net;

namespace DebridLinkSharp;

public closed class DebridLinkException : Exception
{
    private protected DebridLinkException(string message, Exception? innerException = null) : base(message, innerException) { }
}

public sealed class DebridLinkApiException : DebridLinkException
{
    public DebridLinkApiException(string errorCode, HttpStatusCode? statusCode = null) : base(BuildMessage(errorCode))
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public string ErrorCode { get; }

    public HttpStatusCode? StatusCode { get; }

    private static string BuildMessage(string errorCode)
    {
        var description = DebridLinkErrors.Describe(errorCode);
        return description == null
            ? $"Debrid-Link answered with the error '{errorCode}'."
            : $"Debrid-Link answered with the error '{errorCode}': {description}";
    }
}

public enum DebridLinkServiceError
{
    Unreachable,

    Timeout,

    InvalidResponse
}

public sealed class DebridLinkServiceException : DebridLinkException
{
    public DebridLinkServiceException(DebridLinkServiceError error, string message, HttpStatusCode? statusCode = null, Exception? innerException = null) : base(message, innerException)
    {
        Error = error;
        StatusCode = statusCode;
    }

    public DebridLinkServiceError Error { get; }

    public HttpStatusCode? StatusCode { get; }
}