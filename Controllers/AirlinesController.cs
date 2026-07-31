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
    public class AirlinesController : Controller
    {
        private readonly AirportContext _context;

        public AirlinesController(AirportContext context)
        {
            _context = context;
        }

        // GET: Airlines
        public async Task<IActionResult> Index(int? pageNumber, string searchString, string sortOrder)
        {
            const int pageSize = 20;

            var airlines = _context.Airlines.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                airlines = airlines.Where(a =>
                    a.Iata.Contains(searchString) ||
                    (a.Airlinename != null && a.Airlinename.Contains(searchString)));
            }

            airlines = sortOrder switch
            {
                "name_desc" => airlines.OrderByDescending(a => a.Airlinename),
                "iata" => airlines.OrderBy(a => a.Iata),
                "iata_desc" => airlines.OrderByDescending(a => a.Iata),
                _ => airlines.OrderBy(a => a.Airlinename)
            };

            int total = await airlines.CountAsync();
            int page = pageNumber ?? 1;
            if (page < 1) page = 1;

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewData["TotalRecords"] = total;
            ViewData["PageSize"] = pageSize;
            ViewData["SearchString"] = searchString;
            ViewData["SortOrder"] = sortOrder ?? "";

            return View(await airlines.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Airlines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.AirlineId == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airlines/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AirlineId,Iata,Airlinename,BaseAirport")] Airline airline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airline);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airlines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
            {
                return NotFound();
            }
            return View(airline);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AirlineId,Iata,Airlinename,BaseAirport")] Airline airline)
        {
            if (id != airline.AirlineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineExists(airline.AirlineId))
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
            return View(airline);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airlines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.AirlineId == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);
            if (airline != null)
            {
                _context.Airlines.Remove(airline);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirlineExists(int id)
        {
            return _context.Airlines.Any(e => e.AirlineId == id);
        }
    }
}
