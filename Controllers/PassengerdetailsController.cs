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
    public class PassengerdetailsController : Controller
    {
        private readonly AirportContext _context;

        public PassengerdetailsController(AirportContext context)
        {
            _context = context;
        }

        // GET: Passengerdetails
        public async Task<IActionResult> Index()
        {
            var airportContext = _context.Passengerdetails.Include(p => p.Passenger);
            return View(await airportContext.ToListAsync());
        }

        // GET: Passengerdetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passengerdetail = await _context.Passengerdetails
                .Include(p => p.Passenger)
                .FirstOrDefaultAsync(m => m.PassengerId == id);
            if (passengerdetail == null)
            {
                return NotFound();
            }

            return View(passengerdetail);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Passengerdetails/Create
        public IActionResult Create()
        {
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: Passengerdetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PassengerId,Birthdate,Sex,Street,City,Zip,Country,Emailaddress,Telephoneno")] Passengerdetail passengerdetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(passengerdetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", passengerdetail.PassengerId);
            return View(passengerdetail);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Passengerdetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passengerdetail = await _context.Passengerdetails.FindAsync(id);
            if (passengerdetail == null)
            {
                return NotFound();
            }
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", passengerdetail.PassengerId);
            return View(passengerdetail);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Passengerdetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PassengerId,Birthdate,Sex,Street,City,Zip,Country,Emailaddress,Telephoneno")] Passengerdetail passengerdetail)
        {
            if (id != passengerdetail.PassengerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(passengerdetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PassengerdetailExists(passengerdetail.PassengerId))
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
            ViewData["PassengerId"] = new SelectList(_context.Passengers, "PassengerId", "PassengerId", passengerdetail.PassengerId);
            return View(passengerdetail);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Passengerdetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passengerdetail = await _context.Passengerdetails
                .Include(p => p.Passenger)
                .FirstOrDefaultAsync(m => m.PassengerId == id);
            if (passengerdetail == null)
            {
                return NotFound();
            }

            return View(passengerdetail);
        }

        [Authorize(Roles = "Administrador")]
        // POST: Passengerdetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var passengerdetail = await _context.Passengerdetails.FindAsync(id);
            if (passengerdetail != null)
            {
                _context.Passengerdetails.Remove(passengerdetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PassengerdetailExists(int id)
        {
            return _context.Passengerdetails.Any(e => e.PassengerId == id);
        }
    }
}
