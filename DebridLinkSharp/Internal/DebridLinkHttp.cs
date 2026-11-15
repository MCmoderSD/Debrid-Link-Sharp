using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace DebridLinkSharp.Internal;

internal sealed class DebridLinkHttp(HttpClient httpClient, string apiKey, DebridLinkClientOptions options)
{
    public async Task<JsonElement> SendAsync(HttpMethod method, string path, HttpContent? content, CancellationToken cancellationToken)
    {
        HttpStatusCode status;
        string body;

        using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            if (options.Timeout != Timeout.InfiniteTimeSpan)
            {
                linked.CancelAfter(options.Timeout);
            }

            try
            {
                using var request = new HttpRequestMessage(method, new Uri(options.BaseUrl, path));
                request.Content = content;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using var response = await httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linked.Token)
                    .ConfigureAwait(false);

                status = response.StatusCode;
                body = await response.Content.ReadAsStringAsync(linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new DebridLinkServiceException(
                    DebridLinkServiceError.Timeout,
                    $"Debrid-Link did not answer within {options.Timeout}.",
                    innerException: ex);
            }
            catch (HttpRequestException ex)
            {
                throw new DebridLinkServiceException(
                    DebridLinkServiceError.Unreachable,
                    $"Debrid-Link could not be reached: {ex.Message}",
                    innerException: ex);
            }
        }

        return ReadValue(status, body);
    }

    public static T Deserialize<T>(JsonElement value, JsonTypeInfo<T> typeInfo, string what) where T : class
    {
        try
        {
            return value.Deserialize(typeInfo) ?? throw new DebridLinkServiceException(DebridLinkServiceError.InvalidResponse, $"Debrid-Link answered with null instead of {what}.");
        }
        catch (JsonException ex)
        {
            throw new DebridLinkServiceException(DebridLinkServiceError.InvalidResponse, $"Debrid-Link answered with {what} that cannot be read: {ex.Message}", innerException: ex);
        }
    }

    private static JsonElement ReadValue(HttpStatusCode status, string body)
    {
        JsonDocument document;

        try
        {
            document = JsonDocument.Parse(body);
        }
        catch (JsonException ex)
        {
            throw new DebridLinkServiceException(
                DebridLinkServiceError.InvalidResponse,
                $"Debrid-Link answered HTTP {(int)status} with something that is not JSON.",
                status,
                ex);
        }

        using (document)
        {
            var root = document.RootElement;

            if (root.ValueKind is JsonValueKind.Object && root.TryGetProperty("success", out var success) && success.ValueKind is JsonValueKind.True)
            {
                if (!root.TryGetProperty("value", out var value))
                {
                    throw new DebridLinkServiceException(DebridLinkServiceError.InvalidResponse, "Debrid-Link answered without a value.", status);
                }

                return value.Clone();
            }

            var code = root.ValueKind is JsonValueKind.Object && root.TryGetProperty("error", out var error) && error.ValueKind is JsonValueKind.String
                ? error.GetString()
                : null;

            throw new DebridLinkApiException(code ?? $"http{(int)status}", status);
        }
    }
}