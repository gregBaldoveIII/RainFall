using System.Net;
using Microsoft.AspNetCore.Mvc;
using RainFall.Application.Helper;
using RainFall.Application.Interface;
using RainFall.Application.Queries;
using RainFall.Domain.Constants;
using RainFall.Domain.Models;

namespace RainFall.API.Controllers;

public class RainFallController : ControllerBase
{
    private readonly IQueryHandler<GetRainfallReadingPerStationQuery, RainfallReadingResponse> _getRainfallReadingPerStationQueryHandler;
    private readonly ILogger<RainFallController> _logger;

    public RainFallController(IQueryHandler<GetRainfallReadingPerStationQuery, RainfallReadingResponse> getRainfallReadingPerStationQueryHandler, 
        ILogger<RainFallController> logger
        )
    {
        _getRainfallReadingPerStationQueryHandler = getRainfallReadingPerStationQueryHandler;
        _logger = logger;
    }

    /// <summary>
    /// Returns all duplicates by fileHash
    /// </summary>
    /// <param name="stationId">Station ID</param>
    /// <param name="count">How many records to be returned</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>200 - The rainfall readings for the specified station.</returns>
    [HttpGet("{stationId:int}")]
    [ProducesResponseType(typeof(List<RainfallReading>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> GetStationReading(int stationId, [FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation(ModelState.Errors());
            return BadRequest(ResponseMessage.Invalid);
        }

        var result = await _getRainfallReadingPerStationQueryHandler.HandleAsync(new GetRainfallReadingPerStationQuery
        {
            StationId = stationId,
            Count = count
        }, cancellationToken);
        
        if (!result.HasError)
        {
            return Ok(result.Readings);
        }

        return result.ErrorDetail.StatusCode switch
        {
            HttpStatusCode.BadRequest => BadRequest(result.ErrorDetail),
            HttpStatusCode.NotFound => NotFound(result.ErrorDetail),
            HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, result.ErrorDetail),
            _ => Ok()
        };
    }
}