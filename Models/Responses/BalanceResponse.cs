using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Account balance information.
/// </summary>
public class BalanceResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Main balance available for operations.
    /// </summary>
    [JsonPropertyName("balance")]
    public decimal balance { get; set; }

    /// <summary>
    /// Referral earnings balance.
    /// </summary>
    [JsonPropertyName("referral")]
    public decimal referral { get; set; }

    /// <summary>
    /// Amount on hold (pending operations).
    /// </summary>
    [JsonPropertyName("hold")]
    public decimal hold { get; set; }
}
