using System.Net;

namespace Aaio.SDK.Exceptions;

/// <summary>
/// Exception thrown when API authentication fails (401/403).
/// </summary>
public class AaioAuthenticationException : AaioApiException {
    /// <summary>
    /// Initializes a new instance of AaioAuthenticationException.
    /// </summary>
    public AaioAuthenticationException(string message, HttpStatusCode statusCode, string? responseBody)
        : base(message, null, statusCode, responseBody) {
    }
}
