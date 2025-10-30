using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Response containing list of SBP-compatible banks.
/// </summary>
public class SbpBanksResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// List of SBP banks.
    /// </summary>
    [JsonPropertyName("list")]
    public List<SbpBank> list { get; set; } = new();
}
