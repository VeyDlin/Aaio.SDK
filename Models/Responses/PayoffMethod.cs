using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Available withdrawal method information.
/// </summary>
public class PayoffMethod {
    /// <summary>
    /// Withdrawal method code name.
    /// </summary>
    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;

    /// <summary>
    /// Minimum withdrawal amount.
    /// </summary>
    [JsonPropertyName("min")]
    public decimal min { get; set; }

    /// <summary>
    /// Maximum withdrawal amount.
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
