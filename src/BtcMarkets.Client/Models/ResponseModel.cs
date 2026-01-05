using System.Net.Http.Headers;

namespace BtcMarkets.Client.Models;

public class ResponseModel
{
    public required HttpResponseHeaders Headers { get; set; }
    public required string Content { get; set; }
}
