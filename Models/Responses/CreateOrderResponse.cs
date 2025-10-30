using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Response from order creation request.
/// </summary>
public class CreateOrderResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Payment URL to redirect customer.
    /// </summary>
    [JsonPropertyName("url")]
    public string? url { get; set; }

    /// <summary>
    /// Order ID in merchant system.
    /// </summary>
    [JsonPropertyName("id")]
    public string? id { get; set; }

    /// <summary>
    /// Error message if creation failed.
    /// </summary>
    [JsonPropertyName("message")]
    public string? message { get; set; }
}
