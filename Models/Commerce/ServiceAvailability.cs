namespace P3Examen_AirportApp.Models.Commerce;

public class ServiceAvailability
{
    public int ServiceAvailabilityId { get; set; }
    public int AirportServiceId { get; set; }
    public AirportService AirportService { get; set; } = null!;
    public int AirportId { get; set; }
    public DateOnly ServiceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public int Capacity { get; set; }
    public int ReservedCount { get; set; }

    public bool Disponible => ReservedCount < Capacity;
}
