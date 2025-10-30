using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Aaio.SDK.Exceptions;

namespace Aaio.SDK.Infrastructure;

/// <summary>
/// Internal HTTP client for AAIO API communication.
/// </summary>
internal class AaioHttpClient {
    private readonly HttpClient httpClient;
    private readonly string apiKey;
    private readonly ILogger? logger;

    private static readonly JsonSerializerOptions jsonOptions = new() {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    /// <summary>
    /// Initializes a new instance of AaioHttpClient.
    /// </summary>
    public AaioHttpClient(HttpClient httpClient, string apiKey, ILogger? logger = null) {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.apiKey = !string.IsNullOrWhiteSpace(apiKey)
            ? apiKey
            : throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        this.logger = logger;

        this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        this.httpClient.DefaultRequestHeaders.Add("X-Api-Key", this.apiKey);
    }

    /// <summary>
    /// Sends GET request to API endpoint.
    /// </summary>
    public async Task<TResponse> GetAsync<TResponse>(
        string path,
        Dictionary<string, string>? queryParams = null,
        CancellationToken cancellationToken = default) {

        var uri = BuildUri(path, queryParams);
        logger?.LogDebug("Sending GET request to {Uri}", uri);

        try {
            var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) {
            logger?.LogError(ex, "HTTP request failed for GET {Path}", path);
            throw new AaioHttpException($"Network error while calling {path}", ex);
        }
        catch (TaskCanceledException ex) {
            logger?.LogWarning(ex, "Request timeout for GET {Path}", path);
            throw new AaioHttpException($"Request timeout for {path}", ex);
        }
    }

    /// <summary>
    /// Sends POST request to API endpoint.
    /// </summary>
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest requestBody,
        CancellationToken cancellationToken = default) {

        logger?.LogDebug("Sending POST request to {Path}", path);

        try {
            var jsonContent = JsonSerializer.Serialize(requestBody, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(path, content, cancellationToken).ConfigureAwait(false);
            return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) {
            logger?.LogError(ex, "HTTP request failed for POST {Path}", path);
            throw new AaioHttpException($"Network error while calling {path}", ex);
        }
        catch (TaskCanceledException ex) {
            logger?.LogWarning(ex, "Request timeout for POST {Path}", path);
            throw new AaioHttpException($"Request timeout for {path}", ex);
        }
    }

    private string BuildUri(string path, Dictionary<string, string>? queryParams) {
        if (queryParams == null || queryParams.Count == 0) {
            return path;
        }

        var queryString = string.Join("&", queryParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{path}?{queryString}";
    }

    private async Task<TResponse> ProcessResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken) {

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) {
            logger?.LogWarning("API returned error status {StatusCode}: {Body}",
                response.StatusCode, responseBody);

            throw response.StatusCode switch {
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                    new AaioAuthenticationException(
                        "Authentication failed. Check your API key.",
                        response.StatusCode,
                        responseBody),

                HttpStatusCode.BadRequest =>
                    new AaioValidationException(
                        "Request validation failed. Check request parameters.",
                        null,
                        responseBody),

                _ => new AaioApiException(
                    $"API request failed with status {response.StatusCode}",
                    null,
                    response.StatusCode,
                    responseBody)
            };
        }

        try {
            var result = JsonSerializer.Deserialize<TResponse>(responseBody, jsonOptions);
            if (result == null) {
                throw new AaioApiException("Failed to deserialize API response");
            }

            return result;
        }
        catch (JsonException ex) {
            logger?.LogError(ex, "Failed to deserialize response: {Body}", responseBody);
            throw new AaioApiException("Invalid JSON response from API", ex);
        }
    }
}
