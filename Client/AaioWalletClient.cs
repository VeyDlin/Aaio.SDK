using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Aaio.SDK.Infrastructure;
using Aaio.SDK.Models.Requests;
using Aaio.SDK.Models.Responses;

namespace Aaio.SDK.Client;

/// <summary>
/// Implementation of AAIO wallet client.
/// </summary>
public class AaioWalletClient : IAaioWalletClient {
    private readonly AaioHttpClient httpClient;
    private readonly ILogger<AaioWalletClient>? logger;

    /// <summary>
    /// Initializes a new instance of AaioWalletClient.
    /// </summary>
    public AaioWalletClient(HttpClient httpClient, string apiKey, ILogger<AaioWalletClient>? logger = null) {
        if (httpClient == null) {
            throw new ArgumentNullException(nameof(httpClient));
        }
        if (string.IsNullOrWhiteSpace(apiKey)) {
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        }

        this.httpClient = new AaioHttpClient(httpClient, apiKey, logger);
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<BalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default) {
        logger?.LogInformation("Fetching account balance");
        return await httpClient.GetAsync<BalanceResponse>(
            "api/poluchenie-balansa",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<CreatePayoffResponse> CreatePayoffAsync(
        CreatePayoffRequest request,
        CancellationToken cancellationToken = default) {

        if (request == null) {
            throw new ArgumentNullException(nameof(request));
        }

        logger?.LogInformation("Creating payoff for amount {Amount} via {Method}",
            request.amount, request.method);

        return await httpClient.PostAsync<CreatePayoffRequest, CreatePayoffResponse>(
            "api/vyvod-sredstv",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PayoffInfo> GetPayoffInfoAsync(
        string myId,
        CancellationToken cancellationToken = default) {

        if (string.IsNullOrWhiteSpace(myId)) {
            throw new ArgumentException("Payoff ID cannot be empty.", nameof(myId));
        }

        logger?.LogInformation("Fetching payoff info for {PayoffId}", myId);

        var queryParams = new Dictionary<string, string> {
            ["my_id"] = myId
        };

        return await httpClient.GetAsync<PayoffInfo>(
            "api/informaciya-o-zayavke-na-vyvod-sredstv",
            queryParams,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<PayoffMethod>> GetPayoffMethodsAsync(CancellationToken cancellationToken = default) {
        logger?.LogInformation("Fetching available payoff methods");

        var response = await httpClient.GetAsync<PayoffMethodsResponse>(
            "api/dostupnye-metody-dlya-vyvoda-sredstv",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.list;
    }

    /// <inheritdoc/>
    public async Task<PayoffRatesResponse> GetPayoffRatesAsync(
        string method,
        decimal amount,
        CancellationToken cancellationToken = default) {

        if (string.IsNullOrWhiteSpace(method)) {
            throw new ArgumentException("Method cannot be empty.", nameof(method));
        }

        logger?.LogInformation("Fetching payoff rates for {Method} with amount {Amount}", method, amount);

        var queryParams = new Dictionary<string, string> {
            ["method"] = method,
            ["amount"] = amount.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };

        return await httpClient.GetAsync<PayoffRatesResponse>(
            "api/kurs-valyut-pri-vyvode-sredstv",
            queryParams,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<SbpBank>> GetSbpBanksAsync(CancellationToken cancellationToken = default) {
        logger?.LogInformation("Fetching SBP banks list");

        var response = await httpClient.GetAsync<SbpBanksResponse>(
            "api/banki-dlya-vyvoda-sredstv-na-sbp",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.list;
    }

    /// <inheritdoc/>
    public bool ValidatePayoffWebhook(PayoffWebhookData webhook, string secretKey) {
        if (webhook == null) {
            throw new ArgumentNullException(nameof(webhook));
        }
        if (string.IsNullOrWhiteSpace(secretKey)) {
            throw new ArgumentException("Secret key cannot be empty.", nameof(secretKey));
        }

        var signatureString = $"{webhook.myId}:{webhook.status}:{webhook.amount}:{secretKey}";
        var computedSign = ComputeSha256Hash(signatureString);

        var isValid = string.Equals(webhook.sign, computedSign, StringComparison.OrdinalIgnoreCase);

        if (!isValid) {
            logger?.LogWarning("Payoff webhook signature validation failed for {PayoffId}", webhook.myId);
        }

        return isValid;
    }

    /// <inheritdoc/>
    public async Task<IpsResponse> GetServiceIpsAsync(CancellationToken cancellationToken = default) {
        logger?.LogInformation("Fetching AAIO service IPs");

        return await httpClient.GetAsync<IpsResponse>(
            "ip-adresa-servisa",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string ComputeSha256Hash(string input) {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
