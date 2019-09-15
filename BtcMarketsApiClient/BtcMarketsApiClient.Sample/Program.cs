using BtcMarketsApiClient.Sample.Models;


namespace BtcMarketsApiClient.Sample
{
    class Program
    {
        private const string BaseUrl = "https://api.btcmarkets.net";
        private const string ApiKey = "add api key here";
        private const string PrivateKey = "add private key here";
        static void Main(string[] args)
        {
            var orders = new Orders(BaseUrl, ApiKey, PrivateKey);

            //Get Orders Sample

            orders.GetOrdersAsync().Wait();


            //Place new Order sample 
            /*
            var newOrder = new NewOrderModel
            {
                MarketId = "BTC-AUD",
                Price = "100.12",
                Amount = "1.034",
                Type = "Limit",
                Side = "Bid"
            };

            orders.PlaceNewOrder(newOrder).Wait();
            */

            //Cancel Order
            /*
                orders.CancelOrder("1224732").Wait();
            */
        }
    }
}
