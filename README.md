# SDK for the aaio.so API (Payment System)

## Creating a Client with a Cart
```csharp
var aaio = new AaioClient("api key");
var merchant = aaio.CreateMerchant("Merchant ID", "Secret 1");
```

## General Information

### Get Payment Methods
```csharp
var paymentMethods = await merchant.GetPaymentMethodsAsync();
```

### Get AAIO Server IP Addresses
```csharp
var ips = await aaio.GetIpsAsync();
```

## Creating a Payment

### Create a New Payment Link
```csharp
var orderId = Guid.NewGuid().ToString();

var paymentUrl = merchant.CreatePayment(new() {
    amount = 100,                          
    orderId = orderId,                     
    description = "My order description",  
    method = "qiwi",                       
    email = "support@email.com",                  
    referral = "123456",             
    currency = "USD",                      
    language = "en"                        
});

Console.WriteLine(paymentUrl);
```

### Waiting for a Payment Event
To wait for a payment event, you can run a waiter:
```csharp
merchant.waiter.AddWaiter(orderId,
    success: (orderId, info) => {
        Console.WriteLine($"Payment {orderId} success");
    },
    error: (orderId, error) => {
        Console.WriteLine($"Payment {orderId} error: {error}");
    }
);
```

By default, the waiter will wait for 3 days, requesting the status starting from the first second and doubling the interval each time. You can change the settings:
```csharp
merchant.waiter.timeout = TimeSpan.FromDays(3);
merchant.waiter.startDelay = TimeSpan.FromSeconds(1);
merchant.waiter.maxDelay = TimeSpan.FromMinutes(5);
```

### Handling Server Restarts or Failures
If your server restarts or crashes, you need to store all created orders somewhere. You can also use the `Startup()` method!

Let's imagine you have a database with all orders. As soon as you create a new order using `CreatePayment()`, you place it in the database with a status of "process". Remember to update the status when an error occurs or it is successfully completed.

Here is how it might look, place this code at the start of your application:
```csharp
merchant.waiter.outsideSuccess += PaymentWaiterSuccess;
merchant.waiter.outsideError += PaymentWaiterError;

using (var context = databaseFactory.Context()) {

    // Finding all orders in the DB that are still being processed
    var processPayments = context.paymentTable
        .Where(x => x.status == PaymentStatus.process)
        .Select(x => x.id)
        .ToList();

    // Startup() calls AddWaiter() for each ID with a reference to outsideSuccess and outsideError
    merchant.waiter.Startup(
        orderIds: processPayments.Select(x => x.ToString()),
        runScatter: TimeSpan.FromMinutes(1) // The runScatter parameter allows you to launch server requests in random order
    );
}
```

## Account Methods

### Get Balance
```csharp
var balances = await aaio.GetBalancesAsync();
```

### Get Payoff Methods
```csharp
var payoffMethods = await aaio.GetPayoffMethodsAsync();
```

### Get Payoff Rates
```csharp
var payoffRates = await aaio.GetPayoffRatesAsync();
```

### Get Available SBP Banks for Payoff
```csharp
var payoffSbpBanks = await aaio.GetPayoffSbpBanksAsync();
```

### Creating a Payoff
```csharp
var payoff = await aaio.CreatePayoffAsync(
    method: "method",
    amount: 100,
    wallet: "wallet",
    payoffId: "payoffId", 
    commissionType: 0
);
```

### Get Payoff Status
```csharp
var payoffStatus = await aaio.GetPayoffInfoAsync(payoff.id, "aaioId");
```

### Validate Payoff
```csharp
var payoffStatus = await aaio.IsValidPayoff(payoffWebhookData, secretKeyPayoff);
```