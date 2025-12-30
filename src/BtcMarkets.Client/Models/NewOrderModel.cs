using Newtonsoft.Json;

namespace BtcMarkets.Client.Models;

public class NewOrderModel
{
    [JsonProperty("marketId")]
    public required string MarketId { get; set; }

    [JsonProperty("price")]
    public required string Price { get; set; }

    [JsonProperty("amount")]
    public required string Amount { get; set; }

    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("side")]
    public required string Side { get; set; }

    [JsonProperty("triggerPrice")]
    public string? TriggerPrice { get; set; }

    [JsonProperty("targetAmount")]
    public string? TargetAmount { get; set; }

    [JsonProperty("timeInForce")]
    public string? TimeInForce { get; set; }

    [JsonProperty("postOnly")]
    public string? PostOnly { get; set; }

    [JsonProperty("selfTrade")]
    public string? SelfTrade { get; set; }

    [JsonProperty("clientOrderId")]
    public string? ClientOrderId { get; set; }
}
