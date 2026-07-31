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
    public class AirportsController : Controller
    {
        private readonly AirportContext _context;

        public AirportsController(AirportContext context)
        {
            _context = context;
        }

        // GET: Airports
        public async Task<IActionResult> Index(int? pageNumber, string searchString, string sortOrder)
        {
            const int pageSize = 20;

            var airports = _context.Airports.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                airports = airports.Where(a =>
                    a.Name.Contains(searchString) ||
                    (a.Iata != null && a.Iata.Contains(searchString)) ||
                    a.Icao.Contains(searchString));
            }

            airports = sortOrder switch
            {
                "name_desc" => airports.OrderByDescending(a => a.Name),
                "iata" => airports.OrderBy(a => a.Iata),
                "iata_desc" => airports.OrderByDescending(a => a.Iata),
                _ => airports.OrderBy(a => a.Name)
            };

            int total = await airports.CountAsync();
            int page = pageNumber ?? 1;
            if (page < 1) page = 1;

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewData["TotalRecords"] = total;
            ViewData["PageSize"] = pageSize;
            ViewData["SearchString"] = searchString;
            ViewData["SortOrder"] = sortOrder ?? "";

            return View(await airports.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airports/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AirportId,Iata,Icao,Name")] Airport airport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airport);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                return NotFound();
            }
            return View(airport);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AirportId,Iata,Icao,Name")] Airport airport)
        {
            if (id != airport.AirportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportExists(airport.AirportId))
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
            return View(airport);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Airports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .FirstOrDefaultAsync(m => m.AirportId == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Airports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airport = await _context.Airports.FindAsync(id);
            if (airport != null)
            {
                _context.Airports.Remove(airport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportExists(int id)
        {
            return _context.Airports.Any(e => e.AirportId == id);
        }
    }
}
