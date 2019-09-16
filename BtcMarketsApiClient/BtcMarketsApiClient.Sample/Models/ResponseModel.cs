using System.Net.Http.Headers;

namespace BtcMarketsApiClient.Sample.Models
{
    public class ResponseModel
    {
        public HttpResponseHeaders Headers { get; set; }
        public string Content { get; set; }
    }
}
