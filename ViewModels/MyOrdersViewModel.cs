namespace P3Examen_AirportApp.ViewModels;

public class MyOrdersViewModel
{
    public List<MyOrderItemViewModel> Orders { get; set; } = new();

    public int TotalCount => Orders.Count;
}

public class MyOrderItemViewModel
{
    public int PurchaseOrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public string DetailsText { get; set; } = string.Empty;

    public string ReservationsText { get; set; } = string.Empty;

    public int? PaymentTransactionId { get; set; }
}
