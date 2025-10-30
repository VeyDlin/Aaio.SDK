using System.Text.Json.Serialization;
using Aaio.SDK.Models.Enums;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Payment order information.
/// </summary>
public class OrderInfo {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Order ID in AAIO system.
    /// </summary>
    [JsonPropertyName("id")]
    public string id { get; set; } = string.Empty;

    /// <summary>
    /// Merchant's order ID.
    /// </summary>
    [JsonPropertyName("order_id")]
    public string orderId { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal amount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string currency { get; set; } = string.Empty;

    /// <summary>
    /// Payment method used.
    /// </summary>
    [JsonPropertyName("method")]
    public string? method { get; set; }

    /// <summary>
    /// Order status.
    /// </summary>
    [JsonPropertyName("status")]
    public OrderStatus status { get; set; }

    /// <summary>
    /// Merchant ID.
    /// </summary>
    [JsonPropertyName("merchant_id")]
    public string merchantId { get; set; } = string.Empty;

    /// <summary>
    /// Order description.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? description { get; set; }

    /// <summary>
    /// Customer email.
    /// </summary>
    [JsonPropertyName("email")]
    public string? email { get; set; }

    /// <summary>
    /// Creation date (Unix timestamp).
    /// </summary>
    [JsonPropertyName("date")]
    public long date { get; set; }

    /// <summary>
    /// Expiration date (Unix timestamp).
    /// </summary>
    [JsonPropertyName("expired_date")]
    public long? expiredDate { get; set; }

    /// <summary>
    /// Completion date (Unix timestamp).
    /// </summary>
    [JsonPropertyName("complete_date")]
    public long? completeDate { get; set; }
}
