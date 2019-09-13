using BtcMarketsApiClient.Sample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BtcMarketsApiClient.Sample
{
    public class Orders
    {
        private readonly ApiClient _apiClient;

        public Orders(string baseUrl, string apiKey, string privateKey)
        {
            _apiClient = new ApiClient(baseUrl, apiKey, privateKey);
        }

        public async Task GetOrdersAsync()
        {
            var orders = await _apiClient.Get("/v3/orders", "status=all&marketId=BTC-AUD&limit=5&after=1");

            Console.WriteLine(orders);
        }

        public async Task PlaceNewOrder(NewOrderModel model)
        {
            var result = await _apiClient.Post("/v3/orders", null, model);
            Console.WriteLine(result);
        }

        public async Task CancelOrder(string id)
        {
            var result = await _apiClient.Delete("/v3/orders", $"id={id}");
            Console.WriteLine(result);
        }
    }
}
