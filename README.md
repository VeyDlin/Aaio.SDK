# Aaio.SDK

[![NuGet](https://img.shields.io/nuget/v/Aaio.SDK.svg)](https://www.nuget.org/packages/Aaio.SDK/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modern, fully-featured .NET SDK for the [AAIO](https://aaio.so) payment gateway API.

## Features

- **Full API Coverage** - Complete support for both Wallet and Business APIs
- **Strongly Typed** - All requests and responses are strongly typed with comprehensive XML documentation
- **Secure** - Built-in SHA256 signature validation for webhooks with optional IP whitelisting
- **Modern Architecture** - Interface-based design with dependency injection support
- **Async/Await** - Modern async/await patterns with `ConfigureAwait(false)` and full cancellation token support
- **Resilient** - Automatic retry with exponential backoff using Polly for transient failures
- **Logging** - Integrated Microsoft.Extensions.Logging support for debugging and monitoring
- **Testable** - Interface-based design for easy mocking and unit testing
- **Production Ready** - Proper HttpClient management via IHttpClientFactory

## Installation

```bash
dotnet add package Aaio.SDK
```

## Quick Start

### For ASP.NET Core Applications (Recommended)

Register the AAIO clients in your `Program.cs`:

```csharp
using Aaio.SDK.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Wallet API client (for balance and withdrawal operations)
builder.Services.AddAaioWalletClient("YOUR_API_KEY");

// Register Business API client (for payment and merchant operations)
builder.Services.AddAaioBusinessClient(
    apiKey: "YOUR_API_KEY",
    merchantId: "YOUR_MERCHANT_ID",
    secretKey1: "YOUR_SECRET_KEY_1",
    secretKey2: "YOUR_SECRET_KEY_2", // Optional
    configureOptions: options => {
        options.enableIpWhitelistCheck = true; // Enable IP validation for webhooks
    });

var app = builder.Build();
```

Then inject and use in your controllers:

```csharp
using Aaio.SDK.Client;
using Aaio.SDK.Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase {
    private readonly IAaioBusinessClient aaioClient;
    private readonly ILogger<PaymentController> logger;

    public PaymentController(IAaioBusinessClient aaioClient, ILogger<PaymentController> logger) {
        this.aaioClient = aaioClient;
        this.logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto) {
        var request = new CreateOrderRequest {
            orderId = Guid.NewGuid().ToString(),
            amount = dto.amount,
            currency = "RUB",
            description = dto.description,
            email = dto.email,
            successUrl = "https://yoursite.com/payment/success",
            failUrl = "https://yoursite.com/payment/fail"
        };

        var response = await aaioClient.CreateOrderAsync(request);

        logger.LogInformation("Payment created: {OrderId}, URL: {Url}", request.orderId, response.url);

        return Ok(new {
            orderId = request.orderId,
            paymentUrl = response.url
        });
    }
}
```

### For Console Applications

```csharp
using Aaio.SDK.Client;
using Aaio.SDK.Models.Requests;

var httpClient = new HttpClient {
    BaseAddress = new Uri("https://aaio.so")
};

var client = new AaioBusinessClient(
    httpClient,
    apiKey: "YOUR_API_KEY",
    merchantId: "YOUR_MERCHANT_ID",
    secretKey1: "YOUR_SECRET_KEY_1");

// Create payment
var orderRequest = new CreateOrderRequest {
    orderId = $"ORDER-{DateTime.UtcNow:yyyyMMddHHmmss}",
    amount = 500.00m,
    currency = "RUB",
    description = "Premium subscription",
    email = "customer@example.com"
};

var order = await client.CreateOrderAsync(orderRequest);
Console.WriteLine($"Payment URL: {order.url}");
Console.WriteLine($"Order ID: {orderRequest.orderId}");

// Wait for payment completion with timeout
try {
    Console.WriteLine("Waiting for payment...");
    var completedOrder = await client.WaitForPaymentAsync(
        orderRequest.orderId,
        timeout: TimeSpan.FromMinutes(30));

    Console.WriteLine($"✓ Payment completed!");
    Console.WriteLine($"  Status: {completedOrder.status}");
    Console.WriteLine($"  Amount: {completedOrder.amount} {completedOrder.currency}");
}
catch (TimeoutException) {
    Console.WriteLine("✗ Payment timeout - customer did not complete payment");
}
catch (InvalidOperationException ex) {
    Console.WriteLine($"✗ Payment failed: {ex.Message}");
}
```

## API Reference

### Wallet API Client

The `IAaioWalletClient` provides access to personal wallet operations:

#### Balance Operations

```csharp
using Aaio.SDK.Client;

// Get account balance
var balance = await walletClient.GetBalanceAsync();
Console.WriteLine($"Main Balance:     {balance.balance:F2} RUB");
Console.WriteLine($"Referral Balance: {balance.referral:F2} RUB");
Console.WriteLine($"On Hold:          {balance.hold:F2} RUB");
Console.WriteLine($"Total Available:  {balance.balance + balance.referral:F2} RUB");
```

#### Withdrawal (Payoff) Operations

```csharp
using Aaio.SDK.Models.Requests;

// Get available withdrawal methods
var methods = await walletClient.GetPayoffMethodsAsync();
foreach (var method in methods) {
    Console.WriteLine($"{method.name}: {method.min}-{method.max} RUB");
    Console.WriteLine($"  Commission: {method.commissionPercent}% + {method.commissionSum} RUB");
}

// Get exchange rates for withdrawal
var rates = await walletClient.GetPayoffRatesAsync(
    method: "card",
    amount: 1000.00m);
Console.WriteLine($"Exchange rate: {rates.rate} {rates.from} → {rates.to}");

// Create withdrawal request
var payoffRequest = new CreatePayoffRequest {
    myId = $"WITHDRAW-{DateTime.UtcNow:yyyyMMddHHmmss}",
    method = "card",
    amount = 1000.00m,
    wallet = "4111111111111111",
    commissionType = 0 // 0 = from balance, 1 = from amount
};

var payoff = await walletClient.CreatePayoffAsync(payoffRequest);

if (payoff.type == "success") {
    Console.WriteLine($"✓ Withdrawal created: {payoff.id}");
    Console.WriteLine($"  Your ID: {payoff.myId}");
}

// Get withdrawal information
var payoffInfo = await walletClient.GetPayoffInfoAsync(payoffRequest.myId);
Console.WriteLine($"Withdrawal status: {payoffInfo.status}");
Console.WriteLine($"Amount: {payoffInfo.amount}");
Console.WriteLine($"Method: {payoffInfo.method}");
Console.WriteLine($"Wallet: {payoffInfo.wallet}");
```

#### SBP (Fast Payment System) Banks

```csharp
// Get list of SBP-compatible banks for withdrawals
var banks = await walletClient.GetSbpBanksAsync();
foreach (var bank in banks) {
    Console.WriteLine($"{bank.code}: {bank.name}");
}
```

#### Webhook Validation

```csharp
using Aaio.SDK.Models.Responses;

// Validate payoff webhook signature
public bool ValidatePayoffWebhook(PayoffWebhookData webhook) {
    return walletClient.ValidatePayoffWebhook(
        webhook,
        secretKey: "YOUR_SECRET_KEY");
}
```

#### Utility Methods

```csharp
// Get AAIO service IP addresses for firewall whitelisting
var ips = await walletClient.GetServiceIpsAsync();
Console.WriteLine("AAIO Server IPs:");
foreach (var ip in ips.ips) {
    Console.WriteLine($"  {ip}");
}
```

### Business API Client

The `IAaioBusinessClient` provides access to merchant and payment operations:

#### Payment Operations

```csharp
using Aaio.SDK.Models.Requests;

// Get available payment methods
var methods = await businessClient.GetPaymentMethodsAsync();
foreach (var method in methods) {
    Console.WriteLine($"{method.name}:");
    Console.WriteLine($"  Min: {method.min} {method.commissionPercent}%");
    Console.WriteLine($"  Max: {method.max}");
}

// Create payment order
var orderRequest = new CreateOrderRequest {
    orderId = "ORDER-12345",
    amount = 250.00m,
    currency = "RUB",                    // RUB, USD, EUR, etc.
    description = "Product purchase",
    method = "card",                      // Optional: specific payment method
    email = "customer@example.com",       // Optional
    phone = "+79001234567",               // Optional
    lang = "ru",                          // Optional: ru, en
    successUrl = "https://yoursite.com/success",
    failUrl = "https://yoursite.com/fail"
};

var order = await businessClient.CreateOrderAsync(orderRequest);
Console.WriteLine($"Payment URL: {order.url}");

// Get order information
var orderInfo = await businessClient.GetOrderInfoAsync("ORDER-12345");
Console.WriteLine($"Order Status: {orderInfo.status}");
Console.WriteLine($"Amount: {orderInfo.amount} {orderInfo.currency}");
Console.WriteLine($"Method: {orderInfo.method}");
Console.WriteLine($"Email: {orderInfo.email}");
Console.WriteLine($"Created: {DateTimeOffset.FromUnixTimeSeconds(orderInfo.date)}");
```

#### Payment Waiting

The SDK provides automatic payment status polling with exponential backoff:

```csharp
// Wait for payment completion (polls with exponential backoff)
var completedOrder = await businessClient.WaitForPaymentAsync(
    orderId: "ORDER-12345",
    timeout: TimeSpan.FromMinutes(30));

// Default configuration:
// - Start delay: 1 second
// - Max delay: 5 minutes
// - Default timeout: 3 days
// - Exponential backoff with jitter

// With cancellation token
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromMinutes(10));

try {
    var order = await businessClient.WaitForPaymentAsync(
        "ORDER-12345",
        timeout: TimeSpan.FromHours(1),
        cancellationToken: cts.Token);

    Console.WriteLine("Payment completed!");
}
catch (OperationCanceledException) {
    Console.WriteLine("Payment wait cancelled");
}
catch (TimeoutException) {
    Console.WriteLine("Payment timeout");
}
```

## Handling Webhooks

### Payment Webhooks

```csharp
using Aaio.SDK.Models.Responses;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase {
    private readonly IAaioBusinessClient businessClient;
    private readonly IConfiguration configuration;
    private readonly ILogger<WebhookController> logger;

    public WebhookController(
        IAaioBusinessClient businessClient,
        IConfiguration configuration,
        ILogger<WebhookController> logger) {
        this.businessClient = businessClient;
        this.configuration = configuration;
        this.logger = logger;
    }

    [HttpPost("payment")]
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookData webhook) {
        var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        logger.LogInformation("Received payment webhook for order {OrderId} from IP {IP}",
            webhook.orderId, requestIp);

        try {
            // Validate webhook signature and optionally check IP whitelist
            var isValid = await businessClient.ValidatePaymentWebhookAsync(
                webhook,
                secretKey: configuration["Aaio:SecretKey1"]!,
                requestIp: requestIp);

            if (!isValid) {
                logger.LogWarning("Invalid webhook signature for order {OrderId}", webhook.orderId);
                return Unauthorized("Invalid signature");
            }

            // Process successful payment
            logger.LogInformation(
                "Payment completed: Order={OrderId}, Amount={Amount} {Currency}, Profit={Profit}, Commission={Commission}",
                webhook.orderId, webhook.amount, webhook.currency, webhook.profit, webhook.commission);

            // Update your database
            // Fulfill order
            // Send confirmation email
            // etc.

            // IMPORTANT: Always return 200 OK
            return Ok("OK");
        }
        catch (AaioSecurityException ex) {
            logger.LogError(ex, "IP whitelist validation failed for {IP}", requestIp);
            return Unauthorized("Untrusted IP");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Webhook processing failed for order {OrderId}", webhook.orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("payoff")]
    public IActionResult PayoffWebhook([FromBody] PayoffWebhookData webhook) {
        var walletClient = HttpContext.RequestServices.GetRequiredService<IAaioWalletClient>();

        var isValid = walletClient.ValidatePayoffWebhook(
            webhook,
            secretKey: configuration["Aaio:SecretKey1"]!);

        if (!isValid) {
            logger.LogWarning("Invalid payoff webhook signature for {PayoffId}", webhook.myId);
            return Unauthorized("Invalid signature");
        }

        logger.LogInformation(
            "Payoff status update: ID={PayoffId}, Status={Status}, Amount={Amount}",
            webhook.myId, webhook.status, webhook.amount);

        // Update withdrawal status in your system

        return Ok("OK");
    }
}
```

### Webhook Signature Validation

The SDK automatically validates webhook signatures using SHA256 HMAC:

```csharp
// Payment webhook signature formula:
// SHA256(merchant_id:amount:currency:secret_key:order_id)

// Payoff webhook signature formula:
// SHA256(my_id:status:amount:secret_key)
```

## Error Handling

The SDK provides a comprehensive exception hierarchy for different error scenarios:

```csharp
using Aaio.SDK.Exceptions;

try {
    var order = await client.CreateOrderAsync(request);
}
catch (AaioAuthenticationException ex) {
    // 401/403 - Invalid API key, unauthorized access
    Console.WriteLine($"Authentication failed: {ex.Message}");
    Console.WriteLine($"Status code: {ex.statusCode}");
    Console.WriteLine($"Response: {ex.responseBody}");
    // Action: Check your API key configuration
}
catch (AaioValidationException ex) {
    // 400 - Invalid request parameters
    Console.WriteLine($"Validation failed: {ex.Message}");
    Console.WriteLine($"Error code: {ex.errorCode}");
    Console.WriteLine($"Response: {ex.responseBody}");
    // Action: Check request parameters (amount, currency, etc.)
}
catch (AaioSecurityException ex) {
    // IP whitelist violation
    Console.WriteLine($"Security check failed: {ex.Message}");
    // Action: Verify webhook is coming from AAIO servers
}
catch (AaioHttpException ex) {
    // Network error, timeout, connection issues
    Console.WriteLine($"HTTP error: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    // Action: Retry the request (SDK does this automatically with Polly)
}
catch (AaioApiException ex) {
    // Generic API error
    Console.WriteLine($"API error: {ex.Message}");
    Console.WriteLine($"Status: {ex.statusCode}");
    Console.WriteLine($"Error code: {ex.errorCode}");
    Console.WriteLine($"Response: {ex.responseBody}");
}
```

### Exception Hierarchy

```
Exception
└── AaioApiException (Base with errorCode, statusCode, responseBody)
    ├── AaioAuthenticationException (401/403 errors)
    ├── AaioValidationException (400 bad request)
    ├── AaioHttpException (Network/timeout errors)
    └── AaioSecurityException (IP whitelist violations)
```

## Configuration Options

### Custom HttpClient Configuration

```csharp
// Custom timeout
builder.Services.AddAaioBusinessClient(
    "api-key",
    "merchant-id",
    "secret-key",
    configureClient: client => {
        client.Timeout = TimeSpan.FromSeconds(60);
        client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    });

// Custom base URL (for testing)
builder.Services.AddAaioWalletClient(
    "api-key",
    configureClient: client => {
        client.BaseAddress = new Uri("https://test.aaio.so");
    });
```

### IP Whitelist Configuration

Enable IP validation for webhooks to ensure they come from AAIO servers:

```csharp
builder.Services.AddAaioBusinessClient(
    "api-key",
    "merchant-id",
    "secret-key",
    configureOptions: options => {
        // Enable IP whitelist check
        options.enableIpWhitelistCheck = true;
    });
```

When enabled, the SDK will:
1. Fetch trusted IP addresses from AAIO API
2. Verify webhook requests originate from these IPs
3. Throw `AaioSecurityException` if IP is not whitelisted

### Logging Configuration

```csharp
// Add console logging
builder.Services.AddLogging(logging => {
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Add Serilog
builder.Host.UseSerilog((context, config) => {
    config.WriteTo.Console()
          .WriteTo.File("logs/aaio-.log", rollingInterval: RollingInterval.Day);
});
```

Log levels used by SDK:
- **Debug**: Request/response details (sanitized)
- **Information**: API calls, successful operations
- **Warning**: Retries, validation failures
- **Error**: HTTP errors, exceptions

### Multiple Merchant Support

Register multiple merchants with different service keys:

```csharp
services.AddHttpClient<AaioBusinessClient>("Merchant1")
    .AddTypedClient((httpClient, sp) => new AaioBusinessClient(
        httpClient,
        "api-key",
        "merchant-1-id",
        "merchant-1-secret"));

services.AddHttpClient<AaioBusinessClient>("Merchant2")
    .AddTypedClient((httpClient, sp) => new AaioBusinessClient(
        httpClient,
        "api-key",
        "merchant-2-id",
        "merchant-2-secret"));

// Usage
public class PaymentService {
    private readonly IHttpClientFactory httpClientFactory;

    public async Task ProcessPayment(string merchantId) {
        var clientName = merchantId == "merchant-1-id" ? "Merchant1" : "Merchant2";
        var client = httpClientFactory.CreateClient(clientName);
        // Use client...
    }
}
```

### Using with IHttpClientFactory

The SDK automatically uses `IHttpClientFactory` when registered via extension methods, ensuring:
- Proper HttpClient lifecycle management
- Connection pooling
- Automatic DNS refresh
- Polly retry policies

## Migration from v0.0.2

Version 1.0.0 is a complete rewrite with significant improvements:

### Key Changes

1. **Architecture**: Split into `AaioWalletClient` and `AaioBusinessClient` instead of unified `AaioClient`
2. **Interfaces**: All clients now have interfaces (`IAaioWalletClient`, `IAaioBusinessClient`)
3. **Dependency Injection**: Use extension methods instead of manual instantiation
4. **HttpClient**: Uses `IHttpClientFactory` for proper lifecycle management
5. **Exceptions**: Custom exception hierarchy instead of generic `Exception`
6. **Property Naming**: Properties use camelCase (orderId, not OrderId)
7. **PaymentWaiter**: Now integrated into BusinessClient with better async support
8. **Logging**: Built-in logging support via Microsoft.Extensions.Logging
9. **Retry**: Automatic retry with Polly for transient failures
10. **Security**: IP whitelist validation for webhooks

### Before (v0.0.2)

```csharp
var aaio = new AaioClient("api-key");
var merchant = aaio.CreateMerchant("merchant-id", "secret");

// Get payment info
var info = await merchant.GetPaymentInfoAsync(orderId);

// Payment waiter (event-based)
merchant.waiter.AddWaiter(orderId,
    success: (id, info) => { /* handle success */ },
    error: (id, error) => { /* handle error */ });
```

### After (v1.0.0)

```csharp
// In Program.cs
services.AddAaioBusinessClient("api-key", "merchant-id", "secret");

// In controller
private readonly IAaioBusinessClient aaioClient;

// Get order info
var info = await aaioClient.GetOrderInfoAsync(orderId);

// Payment waiting (async/await)
try {
    var completedOrder = await aaioClient.WaitForPaymentAsync(
        orderId,
        timeout: TimeSpan.FromMinutes(30));
    // Handle success
}
catch (TimeoutException) {
    // Handle timeout
}
catch (InvalidOperationException) {
    // Handle failure
}
```

## Advanced Examples

### Retry Logic with Polly

The SDK includes automatic retry with exponential backoff by default, but you can customize it:

```csharp
using Polly;
using Polly.Extensions.Http;

services.AddHttpClient<IAaioBusinessClient, AaioBusinessClient>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) => {
                Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s");
            }));
```

### Testing with Mocks

```csharp
using Moq;
using Aaio.SDK.Client;
using Aaio.SDK.Models.Responses;

public class PaymentServiceTests {
    [Fact]
    public async Task CreatePayment_ShouldReturnPaymentUrl() {
        // Arrange
        var mockClient = new Mock<IAaioBusinessClient>();
        mockClient
            .Setup(x => x.CreateOrderAsync(It.IsAny<CreateOrderRequest>(), default))
            .ReturnsAsync(new CreateOrderResponse {
                type = "success",
                url = "https://aaio.so/pay/123"
            });

        var service = new PaymentService(mockClient.Object);

        // Act
        var result = await service.CreatePaymentAsync(100m, "Test");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("aaio.so", result);
    }
}
```

## Documentation

- [Official AAIO API Documentation](https://wiki.aaio.so/)
- [GitHub Repository](https://github.com/VeyDlin/Aaio.SDK)
- [NuGet Package](https://www.nuget.org/packages/Aaio.SDK/)

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Support

For bugs and feature requests, please [open an issue](https://github.com/VeyDlin/Aaio.SDK/issues) on GitHub.

---

Made with ❤️ for the .NET community
