namespace P3Examen_AirportApp.Models.Commerce;

public class AirportService
{
    public int AirportServiceId { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
