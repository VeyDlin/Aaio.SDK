using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Payoff webhook notification data.
/// </summary>
public class PayoffWebhookData {
    /// <summary>
    /// Your payoff identifier.
    /// </summary>
    [JsonPropertyName("my_id")]
    public string myId { get; set; } = string.Empty;

    /// <summary>
    /// Withdrawal status (success/cancelled).
    /// </summary>
    [JsonPropertyName("status")]
    public string status { get; set; } = string.Empty;

    /// <summary>
    /// Withdrawal amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal amount { get; set; }

    /// <summary>
    /// Request signature for validation.
    /// </summary>
    [JsonPropertyName("sign")]
    public string sign { get; set; } = string.Empty;
}
