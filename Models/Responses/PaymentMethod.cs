using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Available payment method information.
/// </summary>
public class PaymentMethod {
    /// <summary>
    /// Payment method code name.
    /// </summary>
    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;

    /// <summary>
    /// Minimum payment amount.
    /// </summary>
    [JsonPropertyName("min")]
    public decimal min { get; set; }

    /// <summary>
    /// Maximum payment amount.
    /// </summary>
    [JsonPropertyName("max")]
    public decimal max { get; set; }

    /// <summary>
    /// Commission percentage.
    /// </summary>
    [JsonPropertyName("commission_percent")]
    public decimal commissionPercent { get; set; }

    /// <summary>
    /// Fixed commission amount.
    /// </summary>
    [JsonPropertyName("commission_sum")]
    public decimal commissionSum { get; set; }
}
