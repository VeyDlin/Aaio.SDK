using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Aaio.SDK.Client;
using Aaio.SDK.Infrastructure;

namespace Aaio.SDK.Extensions;

/// <summary>
/// Extension methods for configuring AAIO SDK services.
/// </summary>
public static class ServiceCollectionExtensions {
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    /// Adds AAIO Wallet Client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">AAIO API key.</param>
    /// <param name="configureClient">Optional HTTP client configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAaioWalletClient(
        this IServiceCollection services,
        string apiKey,
        Action<HttpClient>? configureClient = null) {

        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }
        if (string.IsNullOrWhiteSpace(apiKey)) {
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
        }

        services.AddHttpClient<IAaioWalletClient, AaioWalletClient>((client) => {
            client.BaseAddress = new Uri("https://aaio.so");
            client.Timeout = TimeSpan.FromSeconds(30);
            configureClient?.Invoke(client);
        })
        .AddTypedClient((httpClient, sp) => {
            var logger = sp.GetService<ILogger<AaioWalletClient>>();
            return new AaioWalletClient(httpClient, apiKey, logger);
        })
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    /// <summary>
    /// Adds AAIO Business Client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">AAIO API key.</param>
    /// <param name="merchantId">Merchant ID.</param>
    /// <param name="secretKey1">Secret key #1 for webhook validation.</param>
    /// <param name="secretKey2">Optional secret key #2 for webhook validation.</param>
    /// <param name="configureOptions">Optional webhook options configuration.</param>
    /// <param name="configureClient">Optional HTTP client configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAaioBusinessClient(
        this IServiceCollection services,
        string apiKey,
        string merchantId,
        string secretKey1,
        string? secretKey2 = null,
        Action<AaioWebhookOptions>? configureOptions = null,
        Action<HttpClient>? configureClient = null) {

        if (services == null) {
            throw new ArgumentNullException(nameof(services));
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

        var webhookOptions = new AaioWebhookOptions();
        configureOptions?.Invoke(webhookOptions);
        services.AddSingleton(webhookOptions);

        services.AddHttpClient<IAaioBusinessClient, AaioBusinessClient>((client) => {
            client.BaseAddress = new Uri("https://aaio.so");
            client.Timeout = TimeSpan.FromSeconds(30);
            configureClient?.Invoke(client);
        })
        .AddTypedClient((httpClient, sp) => {
            var logger = sp.GetService<ILogger<AaioBusinessClient>>();
            var options = sp.GetRequiredService<AaioWebhookOptions>();
            return new AaioBusinessClient(httpClient, apiKey, merchantId, secretKey1, secretKey2, options, logger);
        })
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }
}
