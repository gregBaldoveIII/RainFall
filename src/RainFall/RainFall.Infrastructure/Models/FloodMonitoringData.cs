using System.Text.Json.Serialization;

namespace RainFall.Domain.Models;

public class FloodMonitoringData
{
    public string Context { get; set; } = string.Empty;
    public MetaData Meta { get; set; } = new();
    public List<Reading> Items { get; set; } = new();
}

public class MetaData
{
    public string Publisher { get; set; } = string.Empty;
    public string Licence { get; set; } = string.Empty;
    public string Documentation { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public List<string> HasFormat { get; set; } = new();
    public int Limit { get; set; }
}

public class Reading
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Measure { get; set; } = string.Empty;
    public double Value { get; set; }
}
