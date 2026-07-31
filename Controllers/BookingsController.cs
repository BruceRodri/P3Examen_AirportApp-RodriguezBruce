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
    public class BookingsController : Controller
    {
        private readonly AirportContext _context;

        public BookingsController(AirportContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(int? pageNumber, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            const int pageSize = 20;

            var bookings = _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .AsNoTracking()
                .AsQueryable();

            if (minPrice.HasValue)
            {
                bookings = bookings.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                bookings = bookings.Where(b => b.Price <= maxPrice.Value);
            }

            bookings = sortOrder switch
            {
                "price_desc" => bookings.OrderByDescending(b => b.Price),
                "seat" => bookings.OrderBy(b => b.Seat),
                "flight" => bookings.OrderBy(b => b.FlightId),
                _ => bookings.OrderBy(b => b.BookingId)
            };

            int total = await bookings.CountAsync();
            int page = pageNumber ?? 1;
            if (page < 1) page = 1;

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewData["TotalRecords"] = total;
            ViewData["PageSize"] = pageSize;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SortOrder"] = sortOrder ?? "";

            return View(await bookings.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId");
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,FlightId,Seat,PassengerId,Price")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", booking.FlightId);
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", booking.PassengerId);
            return View(booking);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", booking.FlightId);
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", booking.PassengerId);
            return View(booking);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,FlightId,Seat,PassengerId,Price")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", booking.FlightId);
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", booking.PassengerId);
            return View(booking);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
