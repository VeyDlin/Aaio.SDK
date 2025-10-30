using System.Net;

namespace Aaio.SDK.Exceptions;

/// <summary>
/// Base exception for all AAIO API errors.
/// </summary>
public class AaioApiException : Exception {
    /// <summary>
    /// Error code from AAIO API response.
    /// </summary>
    public int? errorCode { get; }

    /// <summary>
    /// HTTP status code from the response.
    /// </summary>
    public HttpStatusCode? statusCode { get; }

    /// <summary>
    /// Raw response body for debugging.
    /// </summary>
    public string? responseBody { get; }

    /// <summary>
    /// Initializes a new instance of AaioApiException.
    /// </summary>
    public AaioApiException(string message) : base(message) {
    }

    /// <summary>
    /// Initializes a new instance of AaioApiException with error details.
    /// </summary>
    public AaioApiException(string message, int? errorCode, HttpStatusCode? statusCode, string? responseBody)
        : base(message) {
        this.errorCode = errorCode;
        this.statusCode = statusCode;
        this.responseBody = responseBody;
    }

    /// <summary>
    /// Initializes a new instance of AaioApiException with inner exception.
    /// </summary>
    public AaioApiException(string message, Exception innerException)
        : base(message, innerException) {
    }
}
