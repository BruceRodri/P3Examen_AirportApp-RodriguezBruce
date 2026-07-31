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
    public class FlightschedulesController : Controller
    {
        private readonly AirportContext _context;

        public FlightschedulesController(AirportContext context)
        {
            _context = context;
        }

        // GET: Flightschedules
        public async Task<IActionResult> Index()
        {
            var airportContext = _context.Flightschedules.Include(f => f.Airline).Include(f => f.FromNavigation).Include(f => f.ToNavigation);
            return View(await airportContext.ToListAsync());
        }

        // GET: Flightschedules/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules
                .Include(f => f.Airline)
                .Include(f => f.FromNavigation)
                .Include(f => f.ToNavigation)
                .FirstOrDefaultAsync(m => m.Flightno == id);
            if (flightschedule == null)
            {
                return NotFound();
            }

            return View(flightschedule);
        }

        // GET: Flightschedules/Create
        public IActionResult Create()
        {
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId");
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            return View();
        }

        // POST: Flightschedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Flightno,From,To,Departure,Arrival,AirlineId,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday")] Flightschedule flightschedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flightschedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flightschedule.AirlineId);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.To);
            return View(flightschedule);
        }

        // GET: Flightschedules/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules.FindAsync(id);
            if (flightschedule == null)
            {
                return NotFound();
            }
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flightschedule.AirlineId);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.To);
            return View(flightschedule);
        }

        // POST: Flightschedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Flightno,From,To,Departure,Arrival,AirlineId,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday")] Flightschedule flightschedule)
        {
            if (id != flightschedule.Flightno)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightschedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightscheduleExists(flightschedule.Flightno))
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
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flightschedule.AirlineId);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flightschedule.To);
            return View(flightschedule);
        }

        // GET: Flightschedules/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules
                .Include(f => f.Airline)
                .Include(f => f.FromNavigation)
                .Include(f => f.ToNavigation)
                .FirstOrDefaultAsync(m => m.Flightno == id);
            if (flightschedule == null)
            {
                return NotFound();
            }

            return View(flightschedule);
        }

        // POST: Flightschedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var flightschedule = await _context.Flightschedules.FindAsync(id);
            if (flightschedule != null)
            {
                _context.Flightschedules.Remove(flightschedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightscheduleExists(string id)
        {
            return _context.Flightschedules.Any(e => e.Flightno == id);
        }
    }
}
