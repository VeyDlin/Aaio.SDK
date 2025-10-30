namespace Aaio.SDK.Exceptions;

/// <summary>
/// Exception thrown for network or HTTP-level errors.
/// </summary>
public class AaioHttpException : AaioApiException {
    /// <summary>
    /// Initializes a new instance of AaioHttpException.
    /// </summary>
    public AaioHttpException(string message, Exception innerException)
        : base(message, innerException) {
    }
}
