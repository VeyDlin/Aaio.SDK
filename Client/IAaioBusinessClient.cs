using Aaio.SDK.Models.Requests;
using Aaio.SDK.Models.Responses;

namespace Aaio.SDK.Client;

/// <summary>
/// Client for AAIO business operations (payments, orders).
/// </summary>
public interface IAaioBusinessClient {
    /// <summary>
    /// Creates a payment order.
    /// </summary>
    Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order information.
    /// </summary>
    Task<OrderInfo> GetOrderInfoAsync(
        string orderId,
        string? merchantId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets list of available payment methods.
    /// </summary>
    Task<List<PaymentMethod>> GetPaymentMethodsAsync(
        string? merchantId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates payment webhook signature and optionally checks IP whitelist.
    /// </summary>
    Task<bool> ValidatePaymentWebhookAsync(
        PaymentWebhookData webhook,
        string secretKey,
        string? requestIp = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for payment to complete with timeout.
    /// </summary>
    Task<OrderInfo> WaitForPaymentAsync(
        string orderId,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
