using Aaio.SDK.Models.Requests;
using Aaio.SDK.Models.Responses;

namespace Aaio.SDK.Client;

/// <summary>
/// Client for AAIO wallet operations (balance, withdrawals).
/// </summary>
public interface IAaioWalletClient {
    /// <summary>
    /// Gets account balance information.
    /// </summary>
    Task<BalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a withdrawal request.
    /// </summary>
    Task<CreatePayoffResponse> CreatePayoffAsync(
        CreatePayoffRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets withdrawal request information.
    /// </summary>
    Task<PayoffInfo> GetPayoffInfoAsync(
        string myId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets list of available withdrawal methods.
    /// </summary>
    Task<List<PayoffMethod>> GetPayoffMethodsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets currency exchange rates for withdrawal.
    /// </summary>
    Task<PayoffRatesResponse> GetPayoffRatesAsync(
        string method,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets list of SBP-compatible banks.
    /// </summary>
    Task<List<SbpBank>> GetSbpBanksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates payoff webhook signature.
    /// </summary>
    bool ValidatePayoffWebhook(PayoffWebhookData webhook, string secretKey);

    /// <summary>
    /// Gets AAIO service IP addresses for whitelisting.
    /// </summary>
    Task<IpsResponse> GetServiceIpsAsync(CancellationToken cancellationToken = default);
}
