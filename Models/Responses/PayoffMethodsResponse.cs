using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Response containing list of available withdrawal methods.
/// </summary>
public class PayoffMethodsResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// List of available withdrawal methods.
    /// </summary>
    [JsonPropertyName("list")]
    public List<PayoffMethod> list { get; set; } = new();
}
