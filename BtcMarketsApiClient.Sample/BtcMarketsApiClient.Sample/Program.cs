using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcMarketsApiClient.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiClient = new ApiClient();
            apiClient.Get("/v3/orders", "status=open").Wait();
        }
    }
}
