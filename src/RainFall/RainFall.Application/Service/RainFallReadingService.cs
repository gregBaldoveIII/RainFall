using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RainFall.Application.Interface;
using RainFall.Domain.Constants;
using RainFall.Domain.Models;
using RainFall.Infrastructure.Interface;
using RainFall.Infrastructure.Models;

namespace RainFall.Application.Service;

public class RainFallReadingService : IRainFallReadingService
{
    private readonly IEnvironmentAgencyAgent _environmentAgencyAgent;
    private readonly IMapper _mapper;
    private readonly ILogger<RainFallReadingService> _logger;

    public RainFallReadingService(IEnvironmentAgencyAgent environmentAgencyAgent,
        IMapper mapper,
        ILogger<RainFallReadingService> logger)
    {
        _environmentAgencyAgent = environmentAgencyAgent;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RainfallReadingResponse> GetRainfallReadingPerStation(int stationId, int count, CancellationToken ct = default)
    {
        var result = new RainfallReadingResponse();

        // If limit/count is below zero, automatically return a bad request status code
        if (count < 0)
        {
            result.ErrorDetail = new Error
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = ResponseMessage.Invalid
            };

            _logger.LogInformation("Count cannot be lesser than zero.");
            return result;
        }

        var readings = await _environmentAgencyAgent.GetStationReading(stationId, count, ct);

        // Reading result can either be a success or a failure
        switch (readings)
        {
            case StationReadingSuccessResponse readingSuccessResponse:
                {
                    var readingData = readingSuccessResponse.Data?.Items;

                    // This checking is created so that we will have an identifier when to return a Not Found status
                    // since client will always return 200 even if there are no items found
                    if (readingData != null && readingData.Any())
                    {
                        result.Readings = readingData.Select(x => _mapper.Map<RainfallReading>(x)).ToList();
                        return result;
                    }

                    _logger.LogError("No readings found under Station Id {stationId}", stationId);

                    result.ErrorDetail = new Error
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = ResponseMessage.NoReadings
                    };

                    break;
                }
            case StationReadingErrorResponse errorResponse:
                {
                    _logger.LogError("Error getting readings under Station Id {stationId}, Status Code: {statusCode}", stationId, errorResponse.StatusCode);

                    result.ErrorDetail = new Error
                    {
                        StatusCode = errorResponse.StatusCode,
                        Message = errorResponse.StatusCode switch
                        {
                            HttpStatusCode.BadRequest => ResponseMessage.Invalid,
                            _ => ResponseMessage.ServiceError
                        }
                    };

                    break;
                }
        }

        return result;
    }
}