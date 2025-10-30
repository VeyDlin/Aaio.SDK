using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Requests;

/// <summary>
/// Request to create a payment order.
/// </summary>
public class CreateOrderRequest {
    /// <summary>
    /// Merchant ID.
    /// </summary>
    [JsonPropertyName("merchant_id")]
    public string merchantId { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal amount { get; set; }

    /// <summary>
    /// Currency code (default: RUB).
    /// </summary>
    [JsonPropertyName("currency")]
    public string currency { get; set; } = "RUB";

    /// <summary>
    /// Your order identifier.
    /// </summary>
    [JsonPropertyName("order_id")]
    public string orderId { get; set; } = string.Empty;

    /// <summary>
    /// Order description.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? description { get; set; }

    /// <summary>
    /// Specific payment method to use.
    /// </summary>
    [JsonPropertyName("method")]
    public string? method { get; set; }

    /// <summary>
    /// Customer email.
    /// </summary>
    [JsonPropertyName("email")]
    public string? email { get; set; }

    /// <summary>
    /// Customer phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    public string? phone { get; set; }

    /// <summary>
    /// Interface language.
    /// </summary>
    [JsonPropertyName("lang")]
    public string? lang { get; set; }

    /// <summary>
    /// Referral code.
    /// </summary>
    [JsonPropertyName("referral")]
    public string? referral { get; set; }

    /// <summary>
    /// Success redirect URL.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? successUrl { get; set; }

    /// <summary>
    /// Fail redirect URL.
    /// </summary>
    [JsonPropertyName("fail_url")]
    public string? failUrl { get; set; }
}
