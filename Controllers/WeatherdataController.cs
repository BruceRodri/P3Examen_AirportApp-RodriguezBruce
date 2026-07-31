using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models;

namespace P3Examen_AirportApp.Controllers
{
    [Authorize]
    public class WeatherdataController : Controller
    {
        private readonly AirportContext _context;

        public WeatherdataController(AirportContext context)
        {
            _context = context;
        }

        // GET: Weatherdata
        public async Task<IActionResult> Index(int? pageNumber, string searchString, DateTime? searchDate, string sortOrder)
        {
            const int pageSize = 20;

            var weather = _context.Weatherdata.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString) && int.TryParse(searchString, out int station))
            {
                weather = weather.Where(w => w.Station == station);
            }

            if (searchDate.HasValue)
            {
                weather = weather.Where(w => w.LogDate == DateOnly.FromDateTime(searchDate.Value));
            }

            weather = sortOrder switch
            {
                "station" => weather.OrderBy(w => w.Station).ThenBy(w => w.LogDate).ThenBy(w => w.Time),
                "station_desc" => weather.OrderByDescending(w => w.Station).ThenBy(w => w.LogDate).ThenBy(w => w.Time),
                "date_desc" => weather.OrderByDescending(w => w.LogDate).ThenByDescending(w => w.Time),
                _ => weather.OrderBy(w => w.LogDate).ThenBy(w => w.Time).ThenBy(w => w.Station)
            };

            int total = await weather.CountAsync();
            int page = pageNumber ?? 1;
            if (page < 1) page = 1;

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewData["TotalRecords"] = total;
            ViewData["PageSize"] = pageSize;
            ViewData["SearchString"] = searchString;
            ViewData["SearchDate"] = searchDate?.ToString("yyyy-MM-dd");
            ViewData["SortOrder"] = sortOrder ?? "";

            return View(await weather.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Weatherdata/Details/5
        public async Task<IActionResult> Details(DateOnly logDate, TimeOnly time, int station)
        {
            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == logDate && m.Time == time && m.Station == station);
            if (weatherdatum == null)
            {
                return NotFound();
            }

            return View(weatherdatum);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Weatherdata/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Weatherdata/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogDate,Time,Station,Temp,Humidity,Airpressure,Wind,Winddirection,Weather")] Weatherdatum weatherdatum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weatherdatum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weatherdatum);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Weatherdata/Edit/5
        public async Task<IActionResult> Edit(DateOnly logDate, TimeOnly time, int station)
        {
            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == logDate && m.Time == time && m.Station == station);
            if (weatherdatum == null)
            {
                return NotFound();
            }
            return View(weatherdatum);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Weatherdata/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DateOnly logDate, TimeOnly time, int station, [Bind("LogDate,Time,Station,Temp,Humidity,Airpressure,Wind,Winddirection,Weather")] Weatherdatum weatherdatum)
        {
            if (logDate != weatherdatum.LogDate || time != weatherdatum.Time || station != weatherdatum.Station)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weatherdatum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeatherdatumExists(logDate, time, station))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weatherdatum);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Weatherdata/Delete/5
        public async Task<IActionResult> Delete(DateOnly logDate, TimeOnly time, int station)
        {
            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == logDate && m.Time == time && m.Station == station);
            if (weatherdatum == null)
            {
                return NotFound();
            }

            return View(weatherdatum);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Weatherdata/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DateOnly logDate, TimeOnly time, int station)
        {
            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == logDate && m.Time == time && m.Station == station);
            if (weatherdatum != null)
            {
                _context.Weatherdata.Remove(weatherdatum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeatherdatumExists(DateOnly logDate, TimeOnly time, int station)
        {
            return _context.Weatherdata.Any(e => e.LogDate == logDate && e.Time == time && e.Station == station);
        }
    }
}
