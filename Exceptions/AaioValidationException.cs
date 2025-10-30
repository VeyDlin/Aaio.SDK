using System.Net;

namespace Aaio.SDK.Exceptions;

/// <summary>
/// Exception thrown when request validation fails (400).
/// </summary>
public class AaioValidationException : AaioApiException {
    /// <summary>
    /// Initializes a new instance of AaioValidationException.
    /// </summary>
    public AaioValidationException(string message, int? errorCode, string? responseBody)
        : base(message, errorCode, HttpStatusCode.BadRequest, responseBody) {
    }
}
