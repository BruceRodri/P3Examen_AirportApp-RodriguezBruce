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
    public class AirplaneTypesController : Controller
    {
        private readonly AirportContext _context;

        public AirplaneTypesController(AirportContext context)
        {
            _context = context;
        }

        // GET: AirplaneTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.AirplaneTypes.ToListAsync());
        }

        // GET: AirplaneTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airplaneType = await _context.AirplaneTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (airplaneType == null)
            {
                return NotFound();
            }

            return View(airplaneType);
        }

        // GET: AirplaneTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AirplaneTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeId,Identifier,Description")] AirplaneType airplaneType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airplaneType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airplaneType);
        }

        // GET: AirplaneTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airplaneType = await _context.AirplaneTypes.FindAsync(id);
            if (airplaneType == null)
            {
                return NotFound();
            }
            return View(airplaneType);
        }

        // POST: AirplaneTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeId,Identifier,Description")] AirplaneType airplaneType)
        {
            if (id != airplaneType.TypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airplaneType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirplaneTypeExists(airplaneType.TypeId))
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
            return View(airplaneType);
        }

        // GET: AirplaneTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airplaneType = await _context.AirplaneTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (airplaneType == null)
            {
                return NotFound();
            }

            return View(airplaneType);
        }

        // POST: AirplaneTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airplaneType = await _context.AirplaneTypes.FindAsync(id);
            if (airplaneType != null)
            {
                _context.AirplaneTypes.Remove(airplaneType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirplaneTypeExists(int id)
        {
            return _context.AirplaneTypes.Any(e => e.TypeId == id);
        }
    }
}
