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
    public class FlightsController : Controller
    {
        private readonly AirportContext _context;

        public FlightsController(AirportContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index(int? pageNumber, string searchString, int? airlineId, DateTime? departureDate, string sortOrder)
        {
            const int pageSize = 20;

            var flights = _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.Airplane)
                .Include(f => f.FlightnoNavigation)
                .Include(f => f.FromNavigation)
                .Include(f => f.ToNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                flights = flights.Where(f => f.Flightno.Contains(searchString));
            }

            if (airlineId.HasValue)
            {
                flights = flights.Where(f => f.AirlineId == airlineId.Value);
            }

            if (departureDate.HasValue)
            {
                flights = flights.Where(f => f.Departure.Date == departureDate.Value.Date);
            }

            flights = sortOrder switch
            {
                "flightno" => flights.OrderBy(f => f.Flightno),
                "departure" => flights.OrderBy(f => f.Departure),
                "departure_desc" => flights.OrderByDescending(f => f.Departure),
                "airline" => flights.OrderBy(f => f.Airline.Airlinename),
                _ => flights.OrderBy(f => f.FlightId)
            };

            int total = await flights.CountAsync();
            int page = pageNumber ?? 1;
            if (page < 1) page = 1;

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewData["TotalRecords"] = total;
            ViewData["PageSize"] = pageSize;
            ViewData["SearchString"] = searchString;
            ViewData["AirlineId"] = airlineId;
            ViewData["DepartureDate"] = departureDate?.ToString("yyyy-MM-dd");
            ViewData["SortOrder"] = sortOrder ?? "";
            ViewData["Airlines"] = new SelectList(
                await _context.Airlines.AsNoTracking().OrderBy(a => a.Airlinename).ToListAsync(),
                "AirlineId", "Airlinename", airlineId);

            return View(await flights.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.Airplane)
                .Include(f => f.FlightnoNavigation)
                .Include(f => f.FromNavigation)
                .Include(f => f.ToNavigation)
                .FirstOrDefaultAsync(m => m.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId");
            ViewData["AirplaneId"] = new SelectList(_context.Airplanes, "AirplaneId", "AirplaneId");
            ViewData["Flightno"] = new SelectList(_context.Flightschedules, "Flightno", "Flightno");
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightId,Flightno,From,To,Departure,Arrival,AirlineId,AirplaneId")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flight.AirlineId);
            ViewData["AirplaneId"] = new SelectList(_context.Airplanes, "AirplaneId", "AirplaneId", flight.AirplaneId);
            ViewData["Flightno"] = new SelectList(_context.Flightschedules, "Flightno", "Flightno", flight.Flightno);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.To);
            return View(flight);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flight.AirlineId);
            ViewData["AirplaneId"] = new SelectList(_context.Airplanes, "AirplaneId", "AirplaneId", flight.AirplaneId);
            ViewData["Flightno"] = new SelectList(_context.Flightschedules, "Flightno", "Flightno", flight.Flightno);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.To);
            return View(flight);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightId,Flightno,From,To,Departure,Arrival,AirlineId,AirplaneId")] Flight flight)
        {
            if (id != flight.FlightId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.FlightId))
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
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "AirlineId", "AirlineId", flight.AirlineId);
            ViewData["AirplaneId"] = new SelectList(_context.Airplanes, "AirplaneId", "AirplaneId", flight.AirplaneId);
            ViewData["Flightno"] = new SelectList(_context.Flightschedules, "Flightno", "Flightno", flight.Flightno);
            ViewData["From"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.From);
            ViewData["To"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.To);
            return View(flight);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.Airplane)
                .Include(f => f.FlightnoNavigation)
                .Include(f => f.FromNavigation)
                .Include(f => f.ToNavigation)
                .FirstOrDefaultAsync(m => m.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.FlightId == id);
        }
    }
}
