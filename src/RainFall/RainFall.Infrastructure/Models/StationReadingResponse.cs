using System.Net;
using RainFall.Domain.Models;

namespace RainFall.Infrastructure.Models;

public class StationReadingResponse
{
    
}

public class StationReadingErrorResponse : StationReadingResponse
{
    public HttpStatusCode StatusCode { get; }

    public StationReadingErrorResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}

public class StationReadingSuccessResponse : StationReadingResponse
{
    public FloodMonitoringData Data { get; }

    public StationReadingSuccessResponse(FloodMonitoringData data)
    {
        Data = data;
    }
}