using System.Text.Json.Serialization;

namespace Aaio.SDK.Models.Enums;

/// <summary>
/// Payment order status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus {
    /// <summary>
    /// Order is pending payment.
    /// </summary>
    in_process,

    /// <summary>
    /// Payment successfully completed.
    /// </summary>
    success,

    /// <summary>
    /// Payment failed or expired.
    /// </summary>
    failed
}
