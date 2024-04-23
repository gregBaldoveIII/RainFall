using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RainFall.Domain.Models;
using RainFall.Infrastructure.Interface;
using RainFall.Infrastructure.Models;

namespace RainFall.Infrastructure.Service;

public class EnvironmentAgencyAgent : IEnvironmentAgencyAgent
{
    private readonly HttpClient _httpClient;
    private readonly EnvironmentAgencyConfiguration _environmentAgencyConfiguration;
    private readonly ILogger<EnvironmentAgencyAgent> _logger;
    
    public EnvironmentAgencyAgent(HttpClient httpClient, 
        IOptions<EnvironmentAgencyConfiguration> environmentAgencyConfiguration,
        ILogger<EnvironmentAgencyAgent> logger)
    {
        _httpClient = httpClient;
        _environmentAgencyConfiguration = environmentAgencyConfiguration.Value;
        _logger = logger;
    }

    public async Task<StationReadingResponse?> GetStationReading(int stationId, int count, CancellationToken ct = default)
    {
        var baseUrl = _environmentAgencyConfiguration.BaseUrl;
        
        var stationPath = $"flood-monitoring/id/stations/{stationId}/readings";
        var limitQuery = $"_limit={count}";
        var requestUrl = $"{baseUrl}{stationPath}?{limitQuery}";
        
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        var response = await _httpClient.SendAsync(request, ct);

        // Also handles scenario wherein no reading is found under a station 
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var stationData = await GetResponse<FloodMonitoringData>(response.Content, ct);
            return new StationReadingSuccessResponse(stationData!);
        }

        _logger.LogError("Error getting readings from external API, Status Code: {statusCode}, Error: {error} ", response.StatusCode,
            await response.Content.ReadAsStringAsync(ct));
        
        var error = await GetResponse<StationReadingErrorResponse>(response.Content, ct);
        return new StationReadingErrorResponse(error!.StatusCode);
    }

    private static async Task<T?> GetResponse<T>(HttpContent content, CancellationToken ct = default)
    {
        return await content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }
}