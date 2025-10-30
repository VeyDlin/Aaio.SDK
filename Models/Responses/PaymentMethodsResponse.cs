using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// Response containing list of available payment methods.
/// </summary>
public class PaymentMethodsResponse {
    /// <summary>
    /// Response type (success or error).
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// List of available payment methods.
    /// </summary>
    [JsonPropertyName("list")]
    public List<PaymentMethod> list { get; set; } = new();
}
