using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BtcMarkets.Client.Exceptions;
using BtcMarkets.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BtcMarkets.Client;

public class BtcMarketsClient : IBtcMarketsClient
{
    private readonly HttpClient _httpClient;
    private readonly BtcMarketsOptions _options;
    private readonly ILogger<BtcMarketsClient> _logger;

    public BtcMarketsClient(HttpClient httpClient, IOptions<BtcMarketsOptions> options, ILogger<BtcMarketsClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.PrivateKey))
        {
            _logger.LogWarning("BtcMarketsClient initialized with empty API Key or Private Key.");
        }
        
        // Ensure base address is set
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }
    }

    public async Task<ResponseModel> GetOrdersAsync(string status = "all", int limit = 5, string? before = null, string? after = null)
    {
        var queryString = $"status={status}&limit={limit}";
        if (!string.IsNullOrEmpty(before)) queryString += $"&before={before}";
        if (!string.IsNullOrEmpty(after)) queryString += $"&after={after}";

        var response = await SendRequestAsync(HttpMethod.Get, "/v3/orders", queryString, null).ConfigureAwait(false);
        return new ResponseModel
        {
            Headers = response.Headers,
            Content = await response.Content.ReadAsStringAsync().ConfigureAwait(false)
        };
    }

    public async Task<string> PlaceNewOrderAsync(NewOrderModel order)
    {
        _logger.LogInformation("Placing new order: {MarketId} {Side} {Type} {Amount} @ {Price}", 
            order.MarketId, order.Side, order.Type, order.Amount, order.Price);
        
        var response = await SendRequestAsync(HttpMethod.Post, "/v3/orders", null, order).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return content;
    }

    public async Task<string> CancelOrderAsync(string orderId)
    {
        _logger.LogInformation("Cancelling order {OrderId}", orderId);
        
        var response = await SendRequestAsync(HttpMethod.Delete, $"/v3/orders/{orderId}", null, null).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return content;
    }

    public async Task<string> CancelAllOrdersAsync(string marketId = "BTC-AUD")
    {
        _logger.LogInformation("Cancelling all orders for market {MarketId}", marketId);
        
        var queryString = $"marketId={marketId}";
        var response = await SendRequestAsync(HttpMethod.Delete, "/v3/orders", queryString, null).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return content;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string path, string? queryString, object? data)
    {
        var fullPath = !string.IsNullOrEmpty(queryString) ? path + "?" + queryString : path;
        string? stringifiedData = data != null ? JsonConvert.SerializeObject(data) : null;

        using var request = new HttpRequestMessage(method, fullPath);
        
        if (data != null)
        {
            request.Content = new StringContent(stringifiedData ?? string.Empty, Encoding.UTF8, "application/json");
        }

        AddAuthHeaders(request, method.Method, stringifiedData, path);

        try
        {
             var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
             
             if (!response.IsSuccessStatusCode)
             {
                 var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                 _logger.LogError("API Error {StatusCode}: {Content}", response.StatusCode, errorContent);
                 throw new BtcMarketsException($"API Error: {errorContent}", (int)response.StatusCode);
             }

             return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP Request failed");
            throw new BtcMarketsException("HTTP Request failed", ex);
        }
    }

    private void AddAuthHeaders(HttpRequestMessage request, string method, string? data, string path)
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var message = method + path + now.ToString();
        if (!string.IsNullOrEmpty(data))
            message += data;

        string signature = SignMessage(message);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("Accept-Charset", "UTF-8");
        request.Headers.Add("BM-AUTH-APIKEY", _options.ApiKey);
        request.Headers.Add("BM-AUTH-TIMESTAMP", now.ToString());
        request.Headers.Add("BM-AUTH-SIGNATURE", signature);
    }

    private string SignMessage(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        using var hash = new HMACSHA512(Convert.FromBase64String(_options.PrivateKey));
        var hashedInputBytes = hash.ComputeHash(bytes);
        return Convert.ToBase64String(hashedInputBytes);
    }
}
