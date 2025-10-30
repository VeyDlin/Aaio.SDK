using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Response from payoff creation request.
/// </summary>
public class CreatePayoffResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Payoff ID in AAIO system.
    /// </summary>
    [JsonPropertyName("id")]
    public string? id { get; set; }

    /// <summary>
    /// Your payoff identifier.
    /// </summary>
    [JsonPropertyName("my_id")]
    public string? myId { get; set; }

    /// <summary>
    /// Error message if creation failed.
    /// </summary>
    [JsonPropertyName("message")]
    public string? message { get; set; }
}
