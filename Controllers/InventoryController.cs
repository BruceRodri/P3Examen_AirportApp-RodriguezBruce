using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models.Commerce;

namespace P3Examen_AirportApp.Controllers;

[Authorize(Roles = "Administrador")]
public class InventoryController : Controller
{
    private readonly ApplicationDbContext _appContext;

    public InventoryController(ApplicationDbContext appContext)
    {
        _appContext = appContext;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _appContext.AirportServices
            .OrderBy(a => a.Name)
            .ToListAsync();

        return View(items);
    }

    public async Task<IActionResult> Inicializar()
    {
        if (await _appContext.AirportServices.AnyAsync())
        {
            return RedirectToAction(nameof(Index));
        }

        var servicios = new (string Nombre, decimal Precio)[]
        {
            ("Sala VIP", 25m),
            ("Estacionamiento", 10m),
            ("Transporte interno", 5m),
            ("Asistencia prioritaria", 15m),
            ("Acompañamiento", 20m),
            ("Traslado entre terminales", 8m)
        };

        int serviceId = 1;
        foreach (var s in servicios)
        {
            _appContext.AirportServices.Add(new AirportService
            {
                ServiceId = serviceId++,
                Name = s.Nombre,
                UnitPrice = s.Precio,
                Stock = 10,
                IsActive = true
            });
        }

        await _appContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AumentarStock(int id, int cantidad)
    {
        var item = await _appContext.AirportServices.FindAsync(id);
        if (item == null) return NotFound();

        item.Stock += cantidad;
        await _appContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
