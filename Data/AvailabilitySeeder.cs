using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Models.Commerce;

namespace P3Examen_AirportApp.Data;

public static class AvailabilitySeeder
{
    public static async Task GarantizarAsync(
        ApplicationDbContext appContext,
        AirportContext airportContext,
        int? serviceId = null)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);

        IQueryable<ServiceAvailability> query = appContext.ServiceAvailabilities.AsQueryable();
        if (serviceId.HasValue)
        {
            query = query.Where(a => a.AirportServiceId == serviceId.Value);
        }

        if (await query.AnyAsync(a => a.ServiceDate >= hoy))
        {
            return;
        }

        var services = serviceId.HasValue
            ? await appContext.AirportServices
                .Where(s => s.AirportServiceId == serviceId.Value)
                .ToListAsync()
            : await appContext.AirportServices.ToListAsync();

        var airports = await airportContext.Airports
            .OrderBy(a => a.AirportId)
            .Take(5)
            .Select(a => new { a.AirportId })
            .ToListAsync();

        if (services.Count == 0 || airports.Count == 0) return;

        var horarios = new[] { new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(18, 0) };
        int capacidad = 5;

        foreach (var service in services)
        {
            foreach (var airport in airports)
            {
                for (int dia = 0; dia <= 6; dia++)
                {
                    var fecha = hoy.AddDays(dia);
                    foreach (var hora in horarios)
                    {
                        appContext.ServiceAvailabilities.Add(new ServiceAvailability
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

        await appContext.SaveChangesAsync();
    }
}
