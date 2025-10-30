using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Responses;

/// <summary>
/// SBP bank information.
/// </summary>
public class SbpBank {
    /// <summary>
    /// Bank code.
    /// </summary>
    [JsonPropertyName("code")]
    public string code { get; set; } = string.Empty;

    /// <summary>
    /// Bank name.
    /// </summary>
    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;
}
