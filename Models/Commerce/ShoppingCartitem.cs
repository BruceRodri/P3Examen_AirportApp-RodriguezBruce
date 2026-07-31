namespace P3Examen_AirportApp.Models.Commerce;

public class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public int AirportServiceId { get; set; }
    public AirportService AirportService { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
