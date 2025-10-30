using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Payment webhook notification data.
/// </summary>
public class PaymentWebhookData {
    /// <summary>
    /// Merchant ID.
    /// </summary>
    [JsonPropertyName("merchant_id")]
    public string merchantId { get; set; } = string.Empty;

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
    /// Profit amount after commission.
    /// </summary>
    [JsonPropertyName("profit")]
    public decimal profit { get; set; }

    /// <summary>
    /// Commission amount.
    /// </summary>
    [JsonPropertyName("commission")]
    public decimal commission { get; set; }

    /// <summary>
    /// Payment method used.
    /// </summary>
    [JsonPropertyName("method")]
    public string method { get; set; } = string.Empty;

    /// <summary>
    /// Customer email.
    /// </summary>
    [JsonPropertyName("email")]
    public string? email { get; set; }

    /// <summary>
    /// Request signature for validation.
    /// </summary>
    [JsonPropertyName("sign")]
    public string sign { get; set; } = string.Empty;
}
