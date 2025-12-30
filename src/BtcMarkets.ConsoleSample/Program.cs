using BtcMarkets.Client;
using BtcMarkets.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure Logging
builder.Logging.AddConsole();

// Register BtcMarkets Client
builder.Services.AddBtcMarkets(options =>
{
    options.BaseUrl = builder.Configuration["BtcMarkets:BaseUrl"] ?? "https://api.btcmarkets.net";
    options.ApiKey = builder.Configuration["BtcMarkets:ApiKey"] ?? string.Empty;
    options.PrivateKey = builder.Configuration["BtcMarkets:PrivateKey"] ?? string.Empty;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<IBtcMarketsClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("\n--- Starting Order Flow Demo (Library Version) ---");

try 
{
    // 1. Place Order
    var newOrder = new NewOrderModel
    {
        MarketId = "XRP-AUD",
        Price = "1.00",
        Amount = "15",
        Type = "Limit",
        Side = "Bid"
    };

    Console.WriteLine("Placing Order...");
    var placeResultJson = await client.PlaceNewOrderAsync(newOrder);
    
    // Parse
    var placeResult = Newtonsoft.Json.Linq.JObject.Parse(placeResultJson);
    var orderId = placeResult["orderId"]?.ToString();

    if (string.IsNullOrEmpty(orderId))
    {
        logger.LogError("Failed to get Order ID. Exiting demo.");
        return;
    }

    Console.WriteLine($"Order Placed. ID: {orderId}");

    // 2. List Orders
    Console.WriteLine("\nListing Open Orders...");
    var orders = await client.GetOrdersAsync("open");
    Console.WriteLine(orders.Content);

    // 3. Cancel Order
    Console.WriteLine($"\nCanceling Order {orderId}...");
    var cancelResult = await client.CancelOrderAsync(orderId);
    Console.WriteLine(cancelResult);

    Console.WriteLine("\n--- Order Flow Demo Complete ---");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during the demo flow");
}
