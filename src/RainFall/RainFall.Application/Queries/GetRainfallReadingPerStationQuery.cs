using MediatR;
using RainFall.Domain.Models;

namespace RainFall.Application.Queries;

public class GetRainfallReadingPerStationQuery : IRequest<RainfallReadingResponse>
{
    public int StationId { get; set; }
    public int Count { get; set; }
}