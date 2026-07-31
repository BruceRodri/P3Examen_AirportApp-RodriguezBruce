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
    public class AirportReachablesController : Controller
    {
        private readonly AirportContext _context;

        public AirportReachablesController(AirportContext context)
        {
            _context = context;
        }

        // GET: AirportReachables
        public async Task<IActionResult> Index()
        {
            var airportContext = _context.AirportReachables.Include(a => a.Airport);
            return View(await airportContext.ToListAsync());
        }

        // GET: AirportReachables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportReachable = await _context.AirportReachables
                .Include(a => a.Airport)
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airportReachable == null)
            {
                return NotFound();
            }

            return View(airportReachable);
        }

        // GET: AirportReachables/Create
        public IActionResult Create()
        {
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            return View();
        }

        // POST: AirportReachables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AirportId,Hops")] AirportReachable airportReachable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airportReachable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportReachable.AirportId);
            return View(airportReachable);
        }

        // GET: AirportReachables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportReachable = await _context.AirportReachables.FindAsync(id);
            if (airportReachable == null)
            {
                return NotFound();
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportReachable.AirportId);
            return View(airportReachable);
        }

        // POST: AirportReachables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AirportId,Hops")] AirportReachable airportReachable)
        {
            if (id != airportReachable.AirportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airportReachable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportReachableExists(airportReachable.AirportId))
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
            ViewData["AirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", airportReachable.AirportId);
            return View(airportReachable);
        }

        // GET: AirportReachables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airportReachable = await _context.AirportReachables
                .Include(a => a.Airport)
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airportReachable == null)
            {
                return NotFound();
            }

            return View(airportReachable);
        }

        // POST: AirportReachables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airportReachable = await _context.AirportReachables.FindAsync(id);
            if (airportReachable != null)
            {
                _context.AirportReachables.Remove(airportReachable);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportReachableExists(int id)
        {
            return _context.AirportReachables.Any(e => e.AirportId == id);
        }
    }
}
