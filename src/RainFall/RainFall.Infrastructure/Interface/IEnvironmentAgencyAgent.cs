using RainFall.Infrastructure.Models;

namespace RainFall.Infrastructure.Interface;

public interface IEnvironmentAgencyAgent
{
    Task<StationReadingResponse?> GetStationReading(int stationId, int count, CancellationToken ct);
}