namespace P3Examen_AirportApp.Models.Commerce;

public class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public int AirportServiceId { get; set; }
    public AirportService AirportService { get; set; } = null!;
    public int AirportId { get; set; }
    public string AirportName { get; set; } = string.Empty;
    public DateOnly ServiceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
