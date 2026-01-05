using System;

namespace BtcMarkets.Client.Exceptions;

public class BtcMarketsException : Exception
{
    public int? StatusCode { get; }
    
    public BtcMarketsException(string message) : base(message)
    {
    }

    public BtcMarketsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BtcMarketsException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
