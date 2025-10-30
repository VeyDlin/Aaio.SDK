namespace Aaio.SDK.Infrastructure;

/// <summary>
/// Configuration options for webhook validation.
/// </summary>
public class AaioWebhookOptions {
    /// <summary>
    /// Enable IP whitelist validation for webhooks (default: false).
    /// When enabled, webhook requests must originate from AAIO server IPs.
    /// </summary>
    public bool enableIpWhitelistCheck { get; set; } = false;
}
