namespace RainFall.Domain.Models;

public class RainfallReadingResponse
{
    public List<RainfallReading> Readings { get; set; } = new();
    public Error ErrorDetail { get; set; } = new();
    public bool HasError => !string.IsNullOrEmpty(ErrorDetail.Message);
}

public class RainfallReading
{
    public DateTime DateMeasured { get; set; }
    public decimal AmountMeasured { get; set; }
}
