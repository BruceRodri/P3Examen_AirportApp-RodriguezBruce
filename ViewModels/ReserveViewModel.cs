namespace P3Examen_AirportApp.ViewModels;

public class ReserveViewModel
{
    public int AirportServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int? AirportId { get; set; }
    public DateOnly? ServiceDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int Quantity { get; set; } = 1;
    public List<TimeOnly> AvailableTimes { get; set; } = new();
    public bool SlotsShown { get; set; }
    public int? SelectedAirportId { get; set; }
    public DateOnly? SelectedDate { get; set; }
}
