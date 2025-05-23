﻿using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Aaio.SDK.Types;
using System.Security.Cryptography;
using System.Text;

namespace Aaio.SDK;


public class AaioClient {
    private HttpClient client;
    private string apiKey;
    private string baseUrl = "https://aaio.so";



    public AaioClient(string apiKey) {
        this.apiKey = apiKey;

        client = new HttpClient {
            BaseAddress = new Uri(baseUrl)
        };
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
    }



    public AaioMerchant CreateMerchant(string merchantId, string secretKey1, string? secretKey2 = null) => new(this, merchantId, secretKey1, secretKey2);



    public Task<PaymentMethods> GetPaymentMethodsAsync(string merchantId) {
        return CreateRequestAsync<PaymentMethods>("/api/methods-pay", new() {
            { "merchant_id", merchantId }
        });
    }



    public string CreatePayment(string merchantId, PaymentParameters parameters, string secretKey1) {
        return QueryHelpers.AddQueryString($"{baseUrl}/merchant/pay", new Dictionary<string, string?> {
            { "merchant_id", merchantId },
            { "amount", parameters.amount.ToString() },
            { "currency", parameters.currency },
            { "order_id", parameters.orderId },
            { "sign", GenerateSign([
                merchantId,
                parameters.amount.ToString(),
                parameters.currency,
                secretKey1,
                parameters.orderId
            ]) },
            { "desc", parameters.description },
            { "lang", parameters.language },
            { "method", parameters.method },
            { "email", parameters.email },
            { "referral", parameters.referral },
            { "us_key", parameters.usKey }
        });
    }



    public async Task<List<string>> GetIpsAsync() {
        return (await CreateRequestAsync<Ips>("/api/public/ips")).list;
    }



    public Task<PaymentInfo> GetPaymentInfoAsync(string merchantId, string orderId) {
        return CreateRequestAsync<PaymentInfo>("/api/info-pay", new() {
            { "merchant_id", merchantId },
            { "order_id", orderId }
        });
    }



    public bool IsValidPayment(PaymentWebhookData payment, string secretKey2) {
        return GenerateSign([payment.merchantId, payment.amount.ToString(), payment.currency, secretKey2, payment.orderId]) == payment.sign;
    }



    public Task<Balance> GetBalancesAsync() {
        return CreateRequestAsync<Balance>("/api/balance");
    }



    public Task<PayoffMethods> GetPayoffMethodsAsync() {
        return CreateRequestAsync<PayoffMethods>("/api/methods-payoff");
    }



    public Task<PayoffRates> GetPayoffRatesAsync() {
        return CreateRequestAsync<PayoffRates>("/api/rates-payoff");
    }



    public Task<CreatePayoff> CreatePayoffAsync(string method, float amount, string wallet, string payoffId = "", int commissionType = 0) {
        return CreateRequestAsync<CreatePayoff>("/api/create-payoff", new() {
            { "my_id", payoffId },
            { "method", method },
            { "amount", amount.ToString() },
            { "wallet", wallet },
            { "commission_type", commissionType.ToString() }
        });
    }



    public async Task<List<SbpBank>> GetPayoffSbpBanksAsync() {
        return (await CreateRequestAsync<PayoffSbpBanks>("/api/sbp-banks-payoff")).list;
    }



    public async Task<PayoffInfo> GetPayoffInfoAsync(string? payoffId = null, string? aaioId = null) {
        return await CreateRequestAsync<PayoffInfo>("/api/info-payoff", new() {
            { "my_id", payoffId },
            { "id", aaioId }
        });
    }



    public bool IsValidPayoff(PayoffWebhookData payoff, string secretKeyPayoff) {
        return GenerateSign([payoff.id, secretKeyPayoff, payoff.amountDown.ToString()]) == payoff.sign;
    }



    private async Task<T> CreateRequestAsync<T>(string uri, Dictionary<string, string?>? parameters = null) {
        var response = await client.PostAsync(
            requestUri: uri,
            content: new FormUrlEncodedContent(parameters ?? new Dictionary<string, string?>())
        );

        var json = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(json);

        if (responseObject!.type != "success") {
            throw new Exception($"AAIO returns code {responseObject.code} with \"{responseObject.message}\"");
        }

        return JsonConvert.DeserializeObject<T>(json)!;
    }



    public static string GenerateSign(string[] parameters) {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(string.Join(":", parameters)));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}