namespace Aaio.SDK.Exceptions;

/// <summary>
/// Exception thrown when webhook IP validation fails.
/// </summary>
public class AaioSecurityException : AaioApiException {
    /// <summary>
    /// Initializes a new instance of AaioSecurityException.
    /// </summary>
    public AaioSecurityException(string message) : base(message) {
    }
}
