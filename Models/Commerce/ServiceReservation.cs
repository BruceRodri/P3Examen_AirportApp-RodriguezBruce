namespace P3Examen_AirportApp.Models.Commerce;

public class ServiceReservation
{
    public int ServiceReservationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int AirportId { get; set; }
    public string AirportName { get; set; } = string.Empty;
    public int AirportServiceId { get; set; }
    public AirportService AirportService { get; set; } = null!;
    public DateOnly ServiceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
