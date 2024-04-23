using RainFall.Domain.Models;

namespace RainFall.Application.Interface;

public interface IRainFallReadingService
{
    public Task<RainfallReadingResponse> GetRainfallReadingPerStation(int stationId, int count, CancellationToken ct = default);
}   