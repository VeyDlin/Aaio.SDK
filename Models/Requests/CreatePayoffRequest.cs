using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Requests;

/// <summary>
/// Request to create a withdrawal.
/// </summary>
public class CreatePayoffRequest {
    /// <summary>
    /// Your payoff identifier.
    /// </summary>
    [JsonPropertyName("my_id")]
    public string myId { get; set; } = string.Empty;

    /// <summary>
    /// Withdrawal method.
    /// </summary>
    [JsonPropertyName("method")]
    public string method { get; set; } = string.Empty;

    /// <summary>
    /// Withdrawal amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal amount { get; set; }

    /// <summary>
    /// Destination wallet address.
    /// </summary>
    [JsonPropertyName("wallet")]
    public string wallet { get; set; } = string.Empty;

    /// <summary>
    /// Commission type (0 = from balance, 1 = from amount).
    /// </summary>
    [JsonPropertyName("commission_type")]
    public int commissionType { get; set; } = 0;
}
