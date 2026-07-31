namespace P3Examen_AirportApp.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();

    public int ItemCount => Items.Sum(i => i.Quantity);

    public decimal Total => Items.Sum(i => i.Subtotal);
}

public class CartItemViewModel
{
    public int ShoppingCartItemId { get; set; }

    public int AirportServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int AirportId { get; set; }

    public string AirportName { get; set; } = string.Empty;

    public DateOnly ServiceDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
}
