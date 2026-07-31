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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Weatherdata.ToListAsync());
        }

        // GET: Weatherdata/Details/5
        public async Task<IActionResult> Details(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == id);
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
        public async Task<IActionResult> Edit(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weatherdatum = await _context.Weatherdata.FindAsync(id);
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
        public async Task<IActionResult> Edit(DateOnly id, [Bind("LogDate,Time,Station,Temp,Humidity,Airpressure,Wind,Winddirection,Weather")] Weatherdatum weatherdatum)
        {
            if (id != weatherdatum.LogDate)
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
                    if (!WeatherdatumExists(weatherdatum.LogDate))
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
        public async Task<IActionResult> Delete(DateOnly? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weatherdatum = await _context.Weatherdata
                .FirstOrDefaultAsync(m => m.LogDate == id);
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
        public async Task<IActionResult> DeleteConfirmed(DateOnly id)
        {
            var weatherdatum = await _context.Weatherdata.FindAsync(id);
            if (weatherdatum != null)
            {
                _context.Weatherdata.Remove(weatherdatum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeatherdatumExists(DateOnly id)
        {
            return _context.Weatherdata.Any(e => e.LogDate == id);
        }
    }
}
