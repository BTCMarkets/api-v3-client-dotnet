using System.Threading.Tasks;
using BtcMarkets.Client.Models;

namespace BtcMarkets.Client;

public interface IBtcMarketsClient
{
    Task<ResponseModel> GetOrdersAsync(string status = "all", int limit = 5, string? before = null, string? after = null);
    Task<string> PlaceNewOrderAsync(NewOrderModel order);
    Task<string> CancelOrderAsync(string orderId);
    Task<string> CancelAllOrdersAsync(string marketId = "BTC-AUD");
    // Expose raw methods if needed, or keep them internal/protected
}
