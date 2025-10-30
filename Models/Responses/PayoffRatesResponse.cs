using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Currency exchange rates for withdrawal.
/// </summary>
public class PayoffRatesResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Exchange rate value.
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal rate { get; set; }

    /// <summary>
    /// Source currency.
    /// </summary>
    [JsonPropertyName("from")]
    public string from { get; set; } = string.Empty;

    /// <summary>
    /// Target currency.
    /// </summary>
    [JsonPropertyName("to")]
    public string to { get; set; } = string.Empty;
}
