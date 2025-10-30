using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Enums;

/// <summary>
/// Withdrawal (payoff) request status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PayoffStatus {
    /// <summary>
    /// Withdrawal is being processed.
    /// </summary>
    in_process,

    /// <summary>
    /// Withdrawal completed successfully.
    /// </summary>
    success,

    /// <summary>
    /// Withdrawal was cancelled.
    /// </summary>
    cancelled
}
