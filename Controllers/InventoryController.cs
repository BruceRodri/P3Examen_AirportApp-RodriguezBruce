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
    private readonly AirportContext _airportContext;

    public InventoryController(ApplicationDbContext appContext, AirportContext airportContext)
    {
        _appContext = appContext;
        _airportContext = airportContext;
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
        if (!await _appContext.AirportServices.AnyAsync())
        {
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
        }

        if (!await _appContext.ServiceAvailabilities.AnyAsync())
        {
            await SembrarDisponibilidadAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task SembrarDisponibilidadAsync()
    {
        var services = await _appContext.AirportServices
            .OrderBy(s => s.AirportServiceId)
            .ToListAsync();

        var airports = await _airportContext.Airports
            .OrderBy(a => a.AirportId)
            .Take(5)
            .Select(a => new { a.AirportId, a.Name })
            .ToListAsync();

        if (services.Count == 0 || airports.Count == 0) return;

        var horarios = new[] { new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(18, 0) };
        int capacidad = 5;

        foreach (var service in services)
        {
            foreach (var airport in airports)
            {
                for (int dia = 1; dia <= 7; dia++)
                {
                    var fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(dia));
                    foreach (var hora in horarios)
                    {
                        _appContext.ServiceAvailabilities.Add(new ServiceAvailability
                        {
                            AirportServiceId = service.AirportServiceId,
                            AirportId = airport.AirportId,
                            ServiceDate = fecha,
                            StartTime = hora,
                            Capacity = capacidad,
                            ReservedCount = 0
                        });
                    }
                }
            }
        }

        await _appContext.SaveChangesAsync();
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
