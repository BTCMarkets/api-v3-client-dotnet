using BtcMarketsApiClient.Sample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task GetOpenOrdersAsync()
        {
            var orders = await _apiClient.Get("/v3/orders", "status=open");

            Console.WriteLine(orders.Content);
        }

        public async Task GetOrdersAsync()
        {
            var orders = await _apiClient.Get("/v3/orders", "status=all&limit=5");

            Console.WriteLine(orders.Content);
            var hasBefore = orders.Headers.TryGetValues("BM_BEFORE", out IEnumerable<string> befores);
            var hasAfter = orders.Headers.TryGetValues("BM-AFTER", out IEnumerable<string> afters);
            var queryString = $"status=all&marketId=XRP-AUD&limit=5";

            if (hasBefore)
                queryString += $"&before={befores.First()}";

            if (hasAfter)
                queryString += $"&before={afters.First()}";

            orders = await _apiClient.Get("/v3/orders", queryString);

            Console.WriteLine(orders.Content);
        }

        public async Task PlaceNewOrder(NewOrderModel model)
        {
            var result = await _apiClient.Post("/v3/orders", null, model);
            Console.WriteLine(result);
        }

        public async Task CancelOrder(string id)
        {
            var result = await _apiClient.Delete($"/v3/orders/{id}", null);
            Console.WriteLine(result);
        }
    }
}
