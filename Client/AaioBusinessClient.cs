using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Aaio.SDK.Exceptions;
using Aaio.SDK.Infrastructure;
using Aaio.SDK.Models.Requests;
using Aaio.SDK.Models.Responses;

namespace Aaio.SDK.Client;

/// <summary>
/// Implementation of AAIO business client.
/// </summary>
public class AaioBusinessClient : IAaioBusinessClient {
    private readonly AaioHttpClient httpClient;
    private readonly string merchantId;
    private readonly string secretKey1;
    private readonly string? secretKey2;
    private readonly AaioWebhookOptions webhookOptions;
    private readonly ILogger<AaioBusinessClient>? logger;
    private readonly Services.PaymentWaiter paymentWaiter;

    /// <summary>
    /// Initializes a new instance of AaioBusinessClient.
    /// </summary>
    public AaioBusinessClient(
        HttpClient httpClient,
        string apiKey,
        string merchantId,
        string secretKey1,
        string? secretKey2 = null,
        AaioWebhookOptions? webhookOptions = null,
        ILogger<AaioBusinessClient>? logger = null) {

        if (httpClient == null) {
            throw new ArgumentNullException(nameof(httpClient));
        }
        if (string.IsNullOrWhiteSpace(apiKey)) {
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        }
        if (string.IsNullOrWhiteSpace(merchantId)) {
            throw new ArgumentException("Merchant ID cannot be empty.", nameof(merchantId));
        }
        if (string.IsNullOrWhiteSpace(secretKey1)) {
            throw new ArgumentException("Secret key 1 cannot be empty.", nameof(secretKey1));
        }

        this.httpClient = new AaioHttpClient(httpClient, apiKey, logger);
        this.merchantId = merchantId;
        this.secretKey1 = secretKey1;
        this.secretKey2 = secretKey2;
        this.webhookOptions = webhookOptions ?? new AaioWebhookOptions();
        this.logger = logger;
        this.paymentWaiter = new Services.PaymentWaiter(this, logger);
    }

    /// <inheritdoc/>
    public async Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) {

        if (request == null) {
            throw new ArgumentNullException(nameof(request));
        }

        request.merchantId = merchantId;

        logger?.LogInformation("Creating order {OrderId} for amount {Amount} {Currency}",
            request.orderId, request.amount, request.currency);

        return await httpClient.PostAsync<CreateOrderRequest, CreateOrderResponse>(
            "merchant/pay",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OrderInfo> GetOrderInfoAsync(
        string orderId,
        string? merchantId = null,
        CancellationToken cancellationToken = default) {

        if (string.IsNullOrWhiteSpace(orderId)) {
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
        }

        logger?.LogInformation("Fetching order info for {OrderId}", orderId);

        var queryParams = new Dictionary<string, string> {
            ["merchant_id"] = merchantId ?? this.merchantId,
            ["order_id"] = orderId
        };

        return await httpClient.GetAsync<OrderInfo>(
            "api/informaciya-o-zakaze",
            queryParams,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync(
        string? merchantId = null,
        CancellationToken cancellationToken = default) {

        logger?.LogInformation("Fetching available payment methods");

        var queryParams = new Dictionary<string, string> {
            ["merchant_id"] = merchantId ?? this.merchantId
        };

        var response = await httpClient.GetAsync<PaymentMethodsResponse>(
            "api/dostupnye-metody-dlya-sozdaniya-zakaza",
            queryParams,
            cancellationToken).ConfigureAwait(false);

        return response.list;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidatePaymentWebhookAsync(
        PaymentWebhookData webhook,
        string secretKey,
        string? requestIp = null,
        CancellationToken cancellationToken = default) {

        if (webhook == null) {
            throw new ArgumentNullException(nameof(webhook));
        }
        if (string.IsNullOrWhiteSpace(secretKey)) {
            throw new ArgumentException("Secret key cannot be empty.", nameof(secretKey));
        }

        var signatureString = $"{webhook.merchantId}:{webhook.amount}:{webhook.currency}:{secretKey}:{webhook.orderId}";
        var computedSign = ComputeSha256Hash(signatureString);

        var isValid = string.Equals(webhook.sign, computedSign, StringComparison.OrdinalIgnoreCase);

        if (!isValid) {
            logger?.LogWarning("Payment webhook signature validation failed for order {OrderId}", webhook.orderId);
            return false;
        }

        if (webhookOptions.enableIpWhitelistCheck && !string.IsNullOrEmpty(requestIp)) {
            logger?.LogDebug("Validating webhook IP: {IP}", requestIp);

            var ipsResponse = await httpClient.GetAsync<IpsResponse>(
                "ip-adresa-servisa",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!ipsResponse.ips.Contains(requestIp)) {
                logger?.LogError("Webhook from untrusted IP: {IP}", requestIp);
                throw new AaioSecurityException($"Request from untrusted IP: {requestIp}");
            }

            logger?.LogDebug("IP validation passed for {IP}", requestIp);
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<OrderInfo> WaitForPaymentAsync(
        string orderId,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default) {

        return await paymentWaiter.WaitForCompletionAsync(orderId, timeout, cancellationToken)
            .ConfigureAwait(false);
    }

    private static string ComputeSha256Hash(string input) {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
