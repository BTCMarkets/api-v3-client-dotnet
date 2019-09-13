using BtcMarketsApiClient.Sample.Models;
using System.Configuration;

namespace BtcMarketsApiClient.Sample
{
    class Program
    {
        private const string BaseUrl= "https://api.btcm.ngin.io";
        private const string ApiKey = "500f9b73-3b07-44c8-ad58-3cec3de70d63";
        private const string PrivateKey = "BARoopTCi6dBxo/0hFKxcnULQ1Ye221XJgyiCNhYd6uJ6NdLPucKfPT3LKu0yjYva5ehYp6HSTobUiea7wDf2Q==";
        static void Main(string[] args)
        {
            var orders = new Orders(BaseUrl, ApiKey, PrivateKey);

            //Get Orders Sample
            /*
            orders.GetOrdersAsync().Wait();
            */

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
            orders.CancelOrder("1224732").Wait();
        }
    }
}
