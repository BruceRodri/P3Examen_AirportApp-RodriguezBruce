namespace P3Examen_AirportApp.ViewModels;

public class AdminReportViewModel
{
    public int TotalOrders { get; set; }

    public Dictionary<string, int> OrdersByStatus { get; set; } = new();

    public decimal TotalPaidAmount { get; set; }

    public decimal AverageOrderAmount { get; set; }

    public int TotalTransactions { get; set; }

    public Dictionary<string, int> PaymentsByProvider { get; set; } = new();

    public List<ServiceSales> TopServices { get; set; } = new();
}

public class ServiceSales
{
    public string ServiceName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Revenue { get; set; }
}
