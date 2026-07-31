using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models.Commerce;

namespace P3Examen_AirportApp.Controllers;

[Authorize]
public class StoreController : Controller
{
    private readonly ApplicationDbContext _context;

    public StoreController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _context.AirportServices
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(services);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int airportServiceId, int quantity)
    {
        var service = await _context.AirportServices
            .FirstOrDefaultAsync(s => s.AirportServiceId == airportServiceId && s.IsActive);

        if (service == null)
        {
            TempData["Error"] = "El servicio no existe o está inactivo.";
            return RedirectToAction(nameof(Index));
        }

        if (quantity < 1 || quantity > service.Stock)
        {
            TempData["Error"] = "La cantidad solicitada supera el stock disponible.";
            return RedirectToAction(nameof(Index));
        }

        string userEmail = User.Identity!.Name!;

        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.UserEmail == userEmail && c.AirportServiceId == airportServiceId);

        if (cartItem == null)
        {
            _context.ShoppingCartItems.Add(new ShoppingCartItem
            {
                UserEmail = userEmail,
                AirportServiceId = airportServiceId,
                Quantity = quantity
            });
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Cart()
    {
        string userEmail = User.Identity!.Name!;

        var items = await _context.ShoppingCartItems
            .Include(c => c.AirportService)
            .Where(c => c.UserEmail == userEmail)
            .ToListAsync();

        return View(items);
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

        return RedirectToAction("CreateLink", "Payment", new { orderId = order.PurchaseOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> CheckoutPayPalButton()
    {
        var userEmail = User.Identity?.Name ?? "usuario@local";

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

        return RedirectToAction("PayPalButton", "Payment", new { orderId = order.PurchaseOrderId });
    }
}
