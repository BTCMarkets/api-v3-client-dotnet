using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BtcMarkets.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace BtcMarkets.Client.Tests;

public class BtcMarketsClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<BtcMarketsOptions>> _optionsMock;
    private readonly Mock<ILogger<BtcMarketsClient>> _loggerMock;
    private readonly BtcMarketsClient _client;

    public BtcMarketsClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.btcmarkets.net")
        };

        _optionsMock = new Mock<IOptions<BtcMarketsOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(new BtcMarketsOptions
        {
            ApiKey = "test-api-key",
            PrivateKey = "SGVsbG8gV29ybGQ=" // Base64 for "Hello World"
        });

        _loggerMock = new Mock<ILogger<BtcMarketsClient>>();

        _client = new BtcMarketsClient(_httpClient, _optionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetOrdersAsync_ShouldReturnResponseModel_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedContent = "[{\"orderId\": \"123\"}]";
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedContent)
            });

        // Act
        var result = await _client.GetOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedContent, result.Content);
        
        // Verify Headers were added
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Headers.Contains("BM-AUTH-APIKEY") &&
                req.Headers.Contains("BM-AUTH-SIGNATURE") &&
                req.Headers.Contains("BM-AUTH-TIMESTAMP")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task PlaceNewOrderAsync_ShouldSignRequestCorrectly()
    {
        // Arrange
        var newOrder = new NewOrderModel 
        { 
            MarketId = "BTC-AUD", 
            Price = "100000", 
            Amount = "0.1", 
            Type = "Limit", 
            Side = "Bid" 
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"orderId\": \"123\"}")
            });

        // Act
        await _client.PlaceNewOrderAsync(newOrder);

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.Headers.Contains("BM-AUTH-SIGNATURE") 
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
