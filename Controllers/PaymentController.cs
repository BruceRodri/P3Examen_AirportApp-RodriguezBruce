using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models.Commerce;
using P3Examen_AirportApp.Services.Payments;
using P3Examen_AirportApp.ViewModels;

namespace P3Examen_AirportApp.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PayPhoneApiLinkService _payPhoneService;
    private readonly PayPalService _payPalService;

    public PaymentController(
        ApplicationDbContext context,
        PayPhoneApiLinkService payPhoneService,
        PayPalService payPalService)
    {
        _context = context;
        _payPhoneService = payPhoneService;
        _payPalService = payPalService;
    }

    public async Task<IActionResult> CreateLink(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        if (!EsPropietarioOAdmin(order))
        {
            return Forbid();
        }

        if (order.Total < 1.00m)
        {
            TempData["Error"] = "No se puede generar el link porque el total es menor a $1.00.";
            return RedirectToAction("Cart", "Store");
        }

        var existing = await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == order.PurchaseOrderId && p.Status == "Pending");

        if (existing != null)
        {
            return RedirectToAction(nameof(Details), new { id = existing.PaymentTransactionId });
        }

        string clientTransactionId = DateTime.Now.ToString("yyMMddHHmmssfff")[..15];
        string reference = $"Orden Airport #{order.PurchaseOrderId}";

        string link = await _payPhoneService.CreatePaymentLinkAsync(
            order.Total,
            clientTransactionId,
            reference);

        var payment = new PaymentTransaction
        {
            PurchaseOrderId = order.PurchaseOrderId,
            UserId = User.Identity!.Name!,
            ClientTransactionId = clientTransactionId,
            Provider = "PayPhone",
            PayphonePaymentUrl = link,
            AmountInCents = ToCents(order.Total),
            Currency = "USD",
            Status = "Pending"
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> CreatePayPalOrder(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        if (!EsPropietarioOAdmin(order))
        {
            return Forbid();
        }

        if (order.Total < 1.00m)
        {
            TempData["Error"] = "No se puede generar el pago porque el total es menor a $1.00.";
            return RedirectToAction("Cart", "Store");
        }

        var existing = await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == order.PurchaseOrderId && p.Status == "Pending");

        if (existing != null)
        {
            if (!string.IsNullOrWhiteSpace(existing.PayPalApprovalUrl))
            {
                return Redirect(existing.PayPalApprovalUrl);
            }
            return RedirectToAction(nameof(Details), new { id = existing.PaymentTransactionId });
        }

        string reference = $"Orden Airport #{order.PurchaseOrderId}";

        var result = await _payPalService.CreateOrderAsync(
            order.Total,
            reference);

        var payment = new PaymentTransaction
        {
            PurchaseOrderId = order.PurchaseOrderId,
            UserId = User.Identity!.Name!,
            ClientTransactionId = result.OrderId,
            Provider = "PayPal",
            PayPalOrderId = result.OrderId,
            PayPalApprovalUrl = result.ApprovalUrl,
            AmountInCents = ToCents(order.Total),
            Currency = "USD",
            Status = "Pending",
            GatewayResponse = result.RawResponse
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync();

        return Redirect(result.ApprovalUrl);
    }

    public async Task<IActionResult> PayPalButton(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        if (!EsPropietarioOAdmin(order))
        {
            return Forbid();
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayPalButtonOrderJson(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null)
        {
            return Json(new
            {
                success = false,
                message = "Orden no encontrada."
            });
        }

        if (!EsPropietarioOAdmin(order))
        {
            return Json(new
            {
                success = false,
                message = "No tienes permiso para esta orden."
            });
        }

        string reference = $"Orden Airport #{order.PurchaseOrderId}";

        var result = await _payPalService.CreateOrderAsync(
            order.Total,
            reference);

        var payment = new PaymentTransaction
        {
            PurchaseOrderId = order.PurchaseOrderId,
            UserId = User.Identity!.Name!,
            ClientTransactionId = result.OrderId,
            Provider = "PayPalButton",
            PayPalOrderId = result.OrderId,
            PayPalApprovalUrl = result.ApprovalUrl,
            AmountInCents = ToCents(order.Total),
            Currency = "USD",
            Status = "Pending",
            GatewayResponse = result.RawResponse
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            paypalOrderId = result.OrderId,
            paymentTransactionId = payment.PaymentTransactionId
        });
    }

    [HttpPost]
    public async Task<IActionResult> CapturePayPalButtonOrderJson([FromBody] PayPalButtonCaptureRequest request)
    {
        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p =>
                p.PaymentTransactionId == request.PaymentTransactionId &&
                p.PayPalOrderId == request.PayPalOrderId);

        if (payment == null || !EsPropietarioOAdmin(payment.PurchaseOrder))
        {
            return Json(new
            {
                success = false,
                message = "Transacción no encontrada."
            });
        }

        try
        {
            var capture = await _payPalService.CaptureOrderAsync(request.PayPalOrderId);
            await AplicarResultadoCapturaAsync(payment, capture);
        }
        catch (Exception ex)
        {
            payment.Status = "Failed";
            payment.GatewayResponse = ex.Message;
            payment.PurchaseOrder.Status = "Failed";
            await ActualizarReservasAsync(payment.PurchaseOrderId, "Failed");
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            redirectUrl = Url.Action("Details", "Payment", new { id = payment.PaymentTransactionId })
        });
    }

    public async Task<IActionResult> Success(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("PayPal no devolvió token de orden.");
        }

        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.Provider == "PayPal" && p.PayPalOrderId == token);

        if (payment == null) return NotFound();

        if (!EsPropietarioOAdmin(payment.PurchaseOrder))
        {
            return Forbid();
        }

        if (payment.Status == "Approved")
        {
            return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
        }

        try
        {
            var capture = await _payPalService.CaptureOrderAsync(token);
            await AplicarResultadoCapturaAsync(payment, capture);
        }
        catch (Exception ex)
        {
            payment.Status = "Failed";
            payment.GatewayResponse = ex.Message;
            payment.PurchaseOrder.Status = "Failed";
            await ActualizarReservasAsync(payment.PurchaseOrderId, "Failed");
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> Cancel(string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            var payment = await _context.PaymentTransactions
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(p => p.Provider == "PayPal" && p.PayPalOrderId == token);

            if (payment != null && payment.Status == "Pending")
            {
                if (!EsPropietarioOAdmin(payment.PurchaseOrder))
                {
                    return Forbid();
                }

                payment.Status = "Canceled";
                payment.PurchaseOrder.Status = "Canceled";
                await ActualizarReservasAsync(payment.PurchaseOrderId, "Canceled");
                await _context.SaveChangesAsync();
            }
        }

        TempData["Error"] = "El pago con PayPal fue cancelado.";
        return RedirectToAction("Index", "Store");
    }

    public async Task<IActionResult> Details(int id)
    {
        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.PaymentTransactionId == id);

        if (payment == null) return NotFound();

        if (!EsPropietarioOAdmin(payment.PurchaseOrder))
        {
            return Forbid();
        }

        var reservations = await _context.ServiceReservations
            .Include(r => r.AirportService)
            .Where(r => r.PurchaseOrderId == payment.PurchaseOrderId)
            .OrderBy(r => r.ServiceDate)
            .ThenBy(r => r.StartTime)
            .ToListAsync();

        ViewData["Reservations"] = reservations;

        return View(payment);
    }

    public async Task<IActionResult> MyOrders()
    {
        string userEmail = User.Identity!.Name!;

        var orders = await _context.PurchaseOrders
            .Include(o => o.Details)
            .Where(o => o.UserEmail == userEmail)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var orderIds = orders.Select(o => o.PurchaseOrderId).ToList();

        var payments = await _context.PaymentTransactions
            .Where(p => orderIds.Contains(p.PurchaseOrderId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var reservations = await _context.ServiceReservations
            .Where(r => r.PurchaseOrderId.HasValue && orderIds.Contains(r.PurchaseOrderId.Value))
            .ToListAsync();

        var model = new MyOrdersViewModel
        {
            Orders = orders.Select(o => new MyOrderItemViewModel
            {
                PurchaseOrderId = o.PurchaseOrderId,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                Total = o.Total,
                DetailsText = string.Join(", ", o.Details.Select(d => $"{d.ServiceName} x{d.Quantity}")),
                ReservationsText = string.Join(", ", reservations
                    .Where(r => r.PurchaseOrderId == o.PurchaseOrderId)
                    .Select(r => $"{r.AirportName} - {r.ServiceDate:dd/MM/yyyy} {r.StartTime}")),
                PaymentTransactionId = payments
                    .FirstOrDefault(p => p.PurchaseOrderId == o.PurchaseOrderId)
                    ?.PaymentTransactionId
            }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AdminOrders()
    {
        var orders = await _context.PurchaseOrders
            .Include(o => o.Details)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AdminTransactions()
    {
        var transactions = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(transactions);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AdminReport()
    {
        var orders = await _context.PurchaseOrders.ToListAsync();
        var payments = await _context.PaymentTransactions.ToListAsync();
        var details = await _context.PurchaseOrderDetails.ToListAsync();

        var model = new AdminReportViewModel
        {
            TotalOrders = orders.Count,
            OrdersByStatus = orders.GroupBy(o => o.Status)
                .ToDictionary(g => g.Key, g => g.Count()),
            TotalPaidAmount = orders.Where(o => o.Status == "Approved").Sum(o => o.Total),
            AverageOrderAmount = orders.Count > 0 ? orders.Average(o => o.Total) : 0m,
            TotalTransactions = payments.Count,
            PaymentsByProvider = payments.GroupBy(p => p.Provider)
                .ToDictionary(g => g.Key, g => g.Count()),
            TopServices = details
                .GroupBy(d => d.ServiceName)
                .OrderByDescending(g => g.Sum(d => d.Quantity))
                .Take(5)
                .Select(g => new ServiceSales
                {
                    ServiceName = g.Key,
                    Quantity = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Subtotal)
                })
                .ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.PaymentTransactionId == id);

        if (payment == null) return NotFound();

        if (payment.Status != "Approved")
        {
            payment.Status = "Approved";
            payment.ConfirmedAt = DateTime.UtcNow;
            payment.PurchaseOrder.Status = "Approved";
            await DescontarStockAsync(payment.PurchaseOrder);
            await LimpiarCarritoAsync(payment.PurchaseOrderId);
            await ActualizarReservasAsync(payment.PurchaseOrderId, "Approved");
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool EsPropietarioOAdmin(PurchaseOrder order)
    {
        if (order == null) return false;
        return order.UserEmail == User.Identity!.Name ||
               User.IsInRole("Administrador");
    }

    private async Task AplicarResultadoCapturaAsync(PaymentTransaction payment, PayPalCaptureResult capture)
    {
        payment.PayPalCaptureId = capture.CaptureId;
        payment.GatewayResponse = capture.RawResponse;
        payment.ConfirmedAt = DateTime.UtcNow;

        if (capture.Status == "COMPLETED")
        {
            payment.Status = "Approved";
            payment.PurchaseOrder.Status = "Approved";
            await DescontarStockAsync(payment.PurchaseOrder);
            await LimpiarCarritoAsync(payment.PurchaseOrderId);
        }
        else if (capture.Status == "DECLINED")
        {
            payment.Status = "Rejected";
            payment.PurchaseOrder.Status = "Rejected";
        }
        else
        {
            payment.Status = "Failed";
            payment.PurchaseOrder.Status = "Failed";
        }

        await ActualizarReservasAsync(payment.PurchaseOrderId, payment.Status);
    }

    private async Task ActualizarReservasAsync(int? orderId, string status)
    {
        if (!orderId.HasValue) return;

        var reservas = await _context.ServiceReservations
            .Where(r => r.PurchaseOrderId == orderId)
            .ToListAsync();

        foreach (var reserva in reservas)
        {
            reserva.Status = status;

            if (status != "Approved")
            {
                var slot = await _context.ServiceAvailabilities.FirstOrDefaultAsync(a =>
                    a.AirportServiceId == reserva.AirportServiceId
                    && a.AirportId == reserva.AirportId
                    && a.ServiceDate == reserva.ServiceDate
                    && a.StartTime == reserva.StartTime);

                if (slot != null)
                {
                    slot.ReservedCount = Math.Max(0, slot.ReservedCount - reserva.Quantity);
                }
            }
        }
    }

    private async Task LimpiarCarritoAsync(int? orderId)
    {
        if (!orderId.HasValue) return;

        var reservas = await _context.ServiceReservations
            .Where(r => r.PurchaseOrderId == orderId)
            .ToListAsync();

        if (reservas.Count == 0) return;

        var items = await _context.ShoppingCartItems
            .Where(c => c.UserEmail == reservas[0].UserId)
            .ToListAsync();

        var aEliminar = items
            .Where(c => reservas.Any(r =>
                r.AirportServiceId == c.AirportServiceId
                && r.AirportId == c.AirportId
                && r.ServiceDate == c.ServiceDate
                && r.StartTime == c.StartTime))
            .ToList();

        _context.ShoppingCartItems.RemoveRange(aEliminar);
    }

    private async Task DescontarStockAsync(PurchaseOrder order)
    {
        foreach (var detail in order.Details)
        {
            var stock = await _context.AirportServices.FindAsync(detail.AirportServiceId);
            if (stock != null)
            {
                stock.Stock = Math.Max(0, stock.Stock - detail.Quantity);
            }
        }
    }

    private static int ToCents(decimal value)
    {
        return (int)Math.Round(value * 100, MidpointRounding.AwayFromZero);
    }
}

public class PayPalButtonCaptureRequest
{
    public string PayPalOrderId { get; set; } = string.Empty;
    public int PaymentTransactionId { get; set; }
}
