# BTCMarkets .NET 9 Client

A modern, production-ready .NET 9 Class Library and Sample Application for the BTCMarkets V3 API.

## Project Structure

This solution (`BtcMarkets.sln`) is organized into the following components:

* **`src/BtcMarkets.Client`**: The reusable Class Library containing all API logic, models, exceptions, and `ServiceCollection` extensions.
* **`src/BtcMarkets.ConsoleSample`**: A console application demonstrating how to consume the library to place, list, and cancel orders.
* **`tests/BtcMarkets.Client.Tests`**: xUnit test project with unit tests for the core library.

## Features

- **.NET 9**: Built for the latest .NET release.
- **Dependency Injection**: Seamless integration via `services.AddBtcMarkets()`.
- **Robust Error Handling**: Typed `BtcMarketsException` with HTTP status codes.
- **Structured Logging**: Full `ILogger` support.
- **Thread Safety**: Uses `IHttpClientFactory` and per-request `HttpRequestMessage` handling.
- **Async/Await**: Non-blocking I/O with `ConfigureAwait(false)` best practices.

## Getting Started

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A BTCMarkets Account (API Key & Private Key)

### 2. Configuration
Open `src/BtcMarkets.ConsoleSample/appsettings.json` and add your keys:
```json
{
  "BtcMarkets": {
    "ApiKey": "YOUR_API_KEY",
    "PrivateKey": "YOUR_PRIVATE_KEY"
  }
}
```

### 3. Build the Solution
Run from the root directory:
```bash
dotnet build
```

### 4. Run Unit Tests
```bash
dotnet test
```

### 5. Run the Demo
```bash
dotnet run --project src/BtcMarkets.ConsoleSample
```

## Usage in Your App

1. **Add Reference**: Add `BtcMarkets.Client` to your project.
2. **Register Services**: In your `Program.cs` or `Startup.cs`:

```csharp
using BtcMarkets.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddBtcMarkets(options =>
{
    options.BaseUrl = "https://api.btcmarkets.net";
    options.ApiKey = builder.Configuration["BtcMarkets:ApiKey"];
    options.PrivateKey = builder.Configuration["BtcMarkets:PrivateKey"];
});
```

3. **Inject and Use**:

```csharp
public class MyTradingService
{
    private readonly IBtcMarketsClient _client;
    private readonly ILogger<MyTradingService> _logger;

    public MyTradingService(IBtcMarketsClient client, ILogger<MyTradingService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task PlaceOrderAsync()
    {
        try 
        {
            await _client.PlaceNewOrderAsync(new NewOrderModel { ... });
        }
        catch (BtcMarketsException ex)
        {
            _logger.LogError("API Error {Code}: {Message}", ex.StatusCode, ex.Message);
        }
    }
}
```
