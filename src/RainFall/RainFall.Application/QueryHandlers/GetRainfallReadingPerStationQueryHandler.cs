using MediatR;
using RainFall.Application.Interface;
using RainFall.Application.Queries;
using RainFall.Domain.Models;

namespace RainFall.Application.QueryHandlers;

public class GetRainfallReadingPerStationQueryHandler : IRequestHandler<GetRainfallReadingPerStationQuery, RainfallReadingResponse>
{
    private readonly IRainFallReadingService _rainFallReadingService;

    public GetRainfallReadingPerStationQueryHandler(IRainFallReadingService rainFallReadingService)
    {
        _rainFallReadingService = rainFallReadingService;
    }

    public async Task<RainfallReadingResponse> Handle(GetRainfallReadingPerStationQuery query, CancellationToken cancellationToken = default)
    {
        return await _rainFallReadingService.GetRainfallReadingPerStation(query.StationId, query.Count, cancellationToken);
    }
}