using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models;

namespace P3Examen_AirportApp.Controllers
{
    public class AirportGeosController : Controller
    {
        private readonly AirportContext _context;

        public AirportGeosController(AirportContext context)
        {
            _context = context;
        }

        // GET: AirportGeos
        public async Task<IActionResult> Index()
        {
            var airportContext = _context.AirportGeos.Include(a => a.Airport);
            return View(await airportContext.ToListAsync());
        }

        // GET: AirportGeos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportGeo = await _context.AirportGeos
                .Include(a => a.Airport)
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airportGeo == null)
            {
                return NotFound();
            }

            return View(airportGeo);
        }

        // GET: AirportGeos/Create
        public IActionResult Create()
        {
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            return View();
        }

        // POST: AirportGeos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AirportId,Name,City,Country,Latitude,Longitude,Geolocation")] AirportGeo airportGeo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airportGeo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportGeo.AirportId);
            return View(airportGeo);
        }

        // GET: AirportGeos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportGeo = await _context.AirportGeos.FindAsync(id);
            if (airportGeo == null)
            {
                return NotFound();
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportGeo.AirportId);
            return View(airportGeo);
        }

        // POST: AirportGeos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AirportId,Name,City,Country,Latitude,Longitude,Geolocation")] AirportGeo airportGeo)
        {
            if (id != airportGeo.AirportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airportGeo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportGeoExists(airportGeo.AirportId))
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
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportGeo.AirportId);
            return View(airportGeo);
        }

        // GET: AirportGeos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportGeo = await _context.AirportGeos
                .Include(a => a.Airport)
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airportGeo == null)
            {
                return NotFound();
            }

            return View(airportGeo);
        }

        // POST: AirportGeos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airportGeo = await _context.AirportGeos.FindAsync(id);
            if (airportGeo != null)
            {
                _context.AirportGeos.Remove(airportGeo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportGeoExists(int id)
        {
            return _context.AirportGeos.Any(e => e.AirportId == id);
        }
    }
}
