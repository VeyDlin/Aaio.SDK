using System.Text.Json.Serialization;
using Aaio.SDK.Models.Enums;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Withdrawal request information.
/// </summary>
public class PayoffInfo {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// Payoff ID in AAIO system.
    /// </summary>
    [JsonPropertyName("id")]
    public string id { get; set; } = string.Empty;

    /// <summary>
    /// Your payoff identifier.
    /// </summary>
    [JsonPropertyName("my_id")]
    public string myId { get; set; } = string.Empty;

    /// <summary>
    /// Withdrawal amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal amount { get; set; }

    /// <summary>
    /// Withdrawal method.
    /// </summary>
    [JsonPropertyName("method")]
    public string method { get; set; } = string.Empty;

    /// <summary>
    /// Destination wallet address.
    /// </summary>
    [JsonPropertyName("wallet")]
    public string wallet { get; set; } = string.Empty;

    /// <summary>
    /// Payoff status.
    /// </summary>
    [JsonPropertyName("status")]
    public PayoffStatus status { get; set; }

    /// <summary>
    /// Creation date (Unix timestamp).
    /// </summary>
    [JsonPropertyName("date")]
    public long date { get; set; }

    /// <summary>
    /// Completion date (Unix timestamp).
    /// </summary>
    [JsonPropertyName("complete_date")]
    public long? completeDate { get; set; }
}
