using Microsoft.Extensions.Logging;
using Aaio.SDK.Client;
using Aaio.SDK.Models.Enums;
using Aaio.SDK.Models.Responses;

namespace Aaio.SDK.Services;

/// <summary>
/// Service for waiting on payment completion with polling.
/// </summary>
internal class PaymentWaiter {
    private readonly IAaioBusinessClient businessClient;
    private readonly ILogger? logger;

    /// <summary>
    /// Initializes a new instance of PaymentWaiter.
    /// </summary>
    public PaymentWaiter(IAaioBusinessClient businessClient, ILogger? logger = null) {
        this.businessClient = businessClient ?? throw new ArgumentNullException(nameof(businessClient));
        this.logger = logger;
    }

    /// <summary>
    /// Waits for payment to complete with exponential backoff polling.
    /// </summary>
    public async Task<OrderInfo> WaitForCompletionAsync(
        string orderId,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default) {

        if (string.IsNullOrWhiteSpace(orderId)) {
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
        }

        var effectiveTimeout = timeout ?? TimeSpan.FromDays(3);
        var startDelay = TimeSpan.FromSeconds(1);
        var maxDelay = TimeSpan.FromMinutes(5);
        var currentDelay = startDelay;

        var startTime = DateTime.UtcNow;
        logger?.LogInformation("Starting payment wait for order {OrderId} with timeout {Timeout}",
            orderId, effectiveTimeout);

        using var timeoutCts = new CancellationTokenSource(effectiveTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        while (!linkedCts.Token.IsCancellationRequested) {
            try {
                var orderInfo = await businessClient.GetOrderInfoAsync(orderId, cancellationToken: linkedCts.Token)
                    .ConfigureAwait(false);

                if (orderInfo.status == OrderStatus.success) {
                    logger?.LogInformation("Payment completed for order {OrderId} after {Elapsed}",
                        orderId, DateTime.UtcNow - startTime);
                    return orderInfo;
                }

                if (orderInfo.status == OrderStatus.failed) {
                    logger?.LogWarning("Payment failed for order {OrderId}", orderId);
                    throw new InvalidOperationException($"Payment failed for order {orderId}");
                }

                var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));
                var delayWithJitter = currentDelay + jitter;

                logger?.LogDebug("Order {OrderId} still pending, waiting {Delay} before retry",
                    orderId, delayWithJitter);

                await Task.Delay(delayWithJitter, linkedCts.Token).ConfigureAwait(false);

                currentDelay = currentDelay + currentDelay <= maxDelay
                    ? currentDelay + currentDelay
                    : maxDelay;
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested) {
                logger?.LogWarning("Payment wait timeout for order {OrderId} after {Elapsed}",
                    orderId, DateTime.UtcNow - startTime);
                throw new TimeoutException($"Payment wait timeout for order {orderId} after {effectiveTimeout}");
            }
        }

        throw new OperationCanceledException("Payment wait was cancelled");
    }
}
