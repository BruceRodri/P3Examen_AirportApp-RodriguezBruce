using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models.Commerce;
using P3Examen_AirportApp.ViewModels;

namespace P3Examen_AirportApp.Controllers;

[Authorize]
public class StoreController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly AirportContext _airportContext;

    public StoreController(ApplicationDbContext context, AirportContext airportContext)
    {
        _context = context;
        _airportContext = airportContext;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _context.AirportServices
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(services);
    }

    public async Task<IActionResult> Reserve(int id)
    {
        var service = await _context.AirportServices
            .FirstOrDefaultAsync(s => s.AirportServiceId == id && s.IsActive);

        if (service == null) return NotFound();

        await CargarAeropuertosAsync(service.AirportServiceId);

        return View(new ReserveViewModel
        {
            AirportServiceId = service.AirportServiceId,
            ServiceName = service.Name,
            UnitPrice = service.UnitPrice
        });
    }

    [HttpPost]
    public async Task<IActionResult> CheckAvailability(ReserveViewModel model)
    {
        var service = await _context.AirportServices
            .FirstOrDefaultAsync(s => s.AirportServiceId == model.AirportServiceId && s.IsActive);

        if (service == null) return NotFound();

        await CargarAeropuertosAsync(service.AirportServiceId);

        model.ServiceName = service.Name;
        model.UnitPrice = service.UnitPrice;

        if (model.AirportId == null || model.ServiceDate == null)
        {
            TempData["Error"] = "Seleccione un aeropuerto y una fecha para comprobar disponibilidad.";
            return View("Reserve", model);
        }

        model.AvailableTimes = await _context.ServiceAvailabilities
            .Where(a => a.AirportServiceId == service.AirportServiceId
                && a.AirportId == model.AirportId
                && a.ServiceDate == model.ServiceDate
                && a.ReservedCount < a.Capacity)
            .OrderBy(a => a.StartTime)
            .Select(a => a.StartTime)
            .ToListAsync();

        model.SlotsShown = true;
        model.SelectedAirportId = model.AirportId;
        model.SelectedDate = model.ServiceDate;

        return View("Reserve", model);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmReservation(ReserveViewModel model)
    {
        var service = await _context.AirportServices
            .FirstOrDefaultAsync(s => s.AirportServiceId == model.AirportServiceId && s.IsActive);

        if (service == null) return NotFound();

        string userEmail = User.Identity!.Name!;

        if (model.AirportId == null || model.ServiceDate == null || model.StartTime == null)
        {
            TempData["Error"] = "Debe seleccionar aeropuerto, fecha y horario.";
            return RedirectToAction(nameof(Reserve), new { id = model.AirportServiceId });
        }

        if (model.Quantity < 1 || model.Quantity > service.Stock)
        {
            TempData["Error"] = "La cantidad solicitada supera el stock disponible.";
            return RedirectToAction(nameof(Reserve), new { id = model.AirportServiceId });
        }

        var slot = await _context.ServiceAvailabilities.FirstOrDefaultAsync(a =>
            a.AirportServiceId == model.AirportServiceId
            && a.AirportId == model.AirportId
            && a.ServiceDate == model.ServiceDate
            && a.StartTime == model.StartTime);

        if (slot == null || slot.ReservedCount + model.Quantity > slot.Capacity)
        {
            TempData["Error"] = "No hay cupos disponibles para el horario seleccionado.";
            return RedirectToAction(nameof(Reserve), new { id = model.AirportServiceId });
        }

        var airportName = await _airportContext.Airports
            .Where(a => a.AirportId == model.AirportId)
            .Select(a => a.Name)
            .FirstOrDefaultAsync();

        var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c =>
            c.UserEmail == userEmail
            && c.AirportServiceId == model.AirportServiceId
            && c.AirportId == model.AirportId
            && c.ServiceDate == model.ServiceDate
            && c.StartTime == model.StartTime);

        if (cartItem == null)
        {
            _context.ShoppingCartItems.Add(new ShoppingCartItem
            {
                UserEmail = userEmail,
                AirportServiceId = model.AirportServiceId,
                AirportId = model.AirportId.Value,
                AirportName = airportName ?? string.Empty,
                ServiceDate = model.ServiceDate.Value,
                StartTime = model.StartTime.Value,
                Quantity = model.Quantity
            });
        }
        else
        {
            if (cartItem.Quantity + model.Quantity > slot.Capacity - slot.ReservedCount)
            {
                TempData["Error"] = "La cantidad solicitada supera los cupos disponibles.";
                return RedirectToAction(nameof(Reserve), new { id = model.AirportServiceId });
            }

            cartItem.Quantity += model.Quantity;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Cart));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        string userEmail = User.Identity!.Name!;

        var item = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.ShoppingCartItemId == id && c.UserEmail == userEmail);

        if (item != null)
        {
            _context.ShoppingCartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Cart));
    }

    public async Task<IActionResult> Cart()
    {
        string userEmail = User.Identity!.Name!;

        var items = await _context.ShoppingCartItems
            .Include(c => c.AirportService)
            .Where(c => c.UserEmail == userEmail)
            .OrderBy(c => c.AirportService.Name)
            .ThenBy(c => c.ServiceDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();

        var model = new CartViewModel
        {
            Items = items.Select(i => new CartItemViewModel
            {
                ShoppingCartItemId = i.ShoppingCartItemId,
                AirportServiceId = i.AirportServiceId,
                ServiceName = i.AirportService.Name,
                AirportId = i.AirportId,
                AirportName = i.AirportName,
                ServiceDate = i.ServiceDate,
                StartTime = i.StartTime,
                Quantity = i.Quantity,
                UnitPrice = i.AirportService.UnitPrice
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(string provider = "PayPhone")
    {
        string userEmail = User.Identity!.Name!;

        var items = await _context.ShoppingCartItems
            .Include(c => c.AirportService)
            .Where(c => c.UserEmail == userEmail)
            .ToListAsync();

        if (items.Count == 0)
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction(nameof(Cart));
        }

        var order = new PurchaseOrder
        {
            UserEmail = userEmail,
            Status = "Pending",
            Details = items.Select(i => new PurchaseOrderDetail
            {
                AirportServiceId = i.AirportServiceId,
                ServiceName = i.AirportService.Name,
                Quantity = i.Quantity,
                UnitPrice = i.AirportService.UnitPrice,
                Subtotal = i.Quantity * i.AirportService.UnitPrice
            }).ToList()
        };

        order.Total = order.Details.Sum(d => d.Subtotal);

        _context.PurchaseOrders.Add(order);
        _context.ShoppingCartItems.RemoveRange(items);
        await _context.SaveChangesAsync();

        await CrearReservasAsync(order.PurchaseOrderId, userEmail, items);
        await _context.SaveChangesAsync();

        if (provider == "PayPal")
        {
            return RedirectToAction("CreatePayPalOrder", "Payment", new { orderId = order.PurchaseOrderId });
        }

        return RedirectToAction("CreateLink", "Payment", new { orderId = order.PurchaseOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> CheckoutPayPalButton()
    {
        var userEmail = User.Identity!.Name!;

        var cartItems = await _context.ShoppingCartItems
            .Include(c => c.AirportService)
            .Where(c => c.UserEmail == userEmail)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction(nameof(Cart));
        }

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.AirportService.Stock)
            {
                TempData["Error"] = $"Stock insuficiente para {item.AirportService.Name}.";
                return RedirectToAction(nameof(Cart));
            }
        }

        decimal total = cartItems.Sum(item =>
            item.Quantity * item.AirportService.UnitPrice);

        if (total < 1.00m)
        {
            TempData["Error"] = "El monto mínimo para pagar con PayPal Sandbox es de $1.00.";
            return RedirectToAction(nameof(Cart));
        }

        var order = new PurchaseOrder
        {
            UserEmail = userEmail,
            Status = "Pending",
            Total = total
        };

        foreach (var item in cartItems)
        {
            var subtotal = item.Quantity * item.AirportService.UnitPrice;

            order.Details.Add(new PurchaseOrderDetail
            {
                AirportServiceId = item.AirportServiceId,
                ServiceName = item.AirportService.Name,
                Quantity = item.Quantity,
                UnitPrice = item.AirportService.UnitPrice,
                Subtotal = subtotal
            });
        }

        _context.PurchaseOrders.Add(order);
        _context.ShoppingCartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        await CrearReservasAsync(order.PurchaseOrderId, userEmail, cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("PayPalButton", "Payment", new { orderId = order.PurchaseOrderId });
    }

    private async Task CargarAeropuertosAsync(int serviceId)
    {
        var airportIds = await _context.ServiceAvailabilities
            .Where(a => a.AirportServiceId == serviceId && a.ReservedCount < a.Capacity)
            .Select(a => a.AirportId)
            .Distinct()
            .ToListAsync();

        var airports = await _airportContext.Airports
            .Where(a => airportIds.Contains(a.AirportId))
            .OrderBy(a => a.Name)
            .Select(a => new { a.AirportId, a.Name })
            .ToListAsync();

        ViewData["Airports"] = new SelectList(airports, "AirportId", "Name");
    }

    private async Task CrearReservasAsync(int orderId, string userId, IEnumerable<ShoppingCartItem> items)
    {
        foreach (var item in items)
        {
            var slot = await _context.ServiceAvailabilities.FirstOrDefaultAsync(a =>
                a.AirportServiceId == item.AirportServiceId
                && a.AirportId == item.AirportId
                && a.ServiceDate == item.ServiceDate
                && a.StartTime == item.StartTime);

            if (slot != null)
            {
                slot.ReservedCount += item.Quantity;
            }

            _context.ServiceReservations.Add(new ServiceReservation
            {
                UserId = userId,
                AirportId = item.AirportId,
                AirportName = item.AirportName,
                AirportServiceId = item.AirportServiceId,
                ServiceDate = item.ServiceDate,
                StartTime = item.StartTime,
                Quantity = item.Quantity,
                UnitPrice = item.AirportService.UnitPrice,
                Status = "Pending",
                PurchaseOrderId = orderId
            });
        }
    }
}
