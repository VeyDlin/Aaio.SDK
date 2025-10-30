using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// AAIO service IP addresses for whitelisting.
/// </summary>
public class IpsResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// List of service IP addresses.
    /// </summary>
    [JsonPropertyName("ips")]
    public List<string> ips { get; set; } = new();
}
