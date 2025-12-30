namespace BtcMarkets.Client;

public class BtcMarketsOptions
{
    public required string ApiKey { get; set; }
    public required string PrivateKey { get; set; }
    public string BaseUrl { get; set; } = "https://api.btcmarkets.net";
}
