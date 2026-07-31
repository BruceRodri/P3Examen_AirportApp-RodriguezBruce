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
    public class FlightLogsController : Controller
    {
        private readonly AirportContext _context;

        public FlightLogsController(AirportContext context)
        {
            _context = context;
        }

        // GET: FlightLogs
        public async Task<IActionResult> Index()
        {
            var airportContext = _context.FlightLogs.Include(f => f.Flight);
            return View(await airportContext.ToListAsync());
        }

        // GET: FlightLogs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.FlightLogId == id);
            if (flightLog == null)
            {
                return NotFound();
            }

            return View(flightLog);
        }

        // GET: FlightLogs/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId");
            return View();
        }

        // POST: FlightLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightLogId,LogDate,User,FlightId,FlightnoOld,FlightnoNew,FromOld,ToOld,FromNew,ToNew,DepartureOld,ArrivalOld,DepartureNew,ArrivalNew,AirplaneIdOld,AirplaneIdNew,AirlineIdOld,AirlineIdNew,Comment")] FlightLog flightLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flightLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", flightLog.FlightId);
            return View(flightLog);
        }

        // GET: FlightLogs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
            {
                return NotFound();
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", flightLog.FlightId);
            return View(flightLog);
        }

        // POST: FlightLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("FlightLogId,LogDate,User,FlightId,FlightnoOld,FlightnoNew,FromOld,ToOld,FromNew,ToNew,DepartureOld,ArrivalOld,DepartureNew,ArrivalNew,AirplaneIdOld,AirplaneIdNew,AirlineIdOld,AirlineIdNew,Comment")] FlightLog flightLog)
        {
            if (id != flightLog.FlightLogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightLogExists(flightLog.FlightLogId))
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
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", flightLog.FlightId);
            return View(flightLog);
        }

        // GET: FlightLogs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.FlightLogId == id);
            if (flightLog == null)
            {
                return NotFound();
            }

            return View(flightLog);
        }

        // POST: FlightLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog != null)
            {
                _context.FlightLogs.Remove(flightLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightLogExists(long id)
        {
            return _context.FlightLogs.Any(e => e.FlightLogId == id);
        }
    }
}
