using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VendingNEA_0.Models;

namespace VendingNEA_0.Controllers
{
    public class CondicionMontoController : Controller
    {
        private readonly VendingContext _context;

        public CondicionMontoController(VendingContext context)
        {
            _context = context;
        }

        // GET: CondicionMonto
        public async Task<IActionResult> Index()
        {
            var vendingContext = _context.CondicionMontos.Include(c => c.CodCondicionNavigation);
            return View(await vendingContext.ToListAsync());
        }

        // GET: CondicionMonto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionMonto = await _context.CondicionMontos
                .Include(c => c.CodCondicionNavigation)
                .FirstOrDefaultAsync(m => m.CodCondicion == id);
            if (condicionMonto == null)
            {
                return NotFound();
            }

            return View(condicionMonto);
        }

        // GET: CondicionMonto/Create
        public IActionResult Create()
        {
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion");
            return View();
        }

        // POST: CondicionMonto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodCondicion,Monto")] CondicionMonto condicionMonto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(condicionMonto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionMonto.CodCondicion);
            return View(condicionMonto);
        }

        // GET: CondicionMonto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionMonto = await _context.CondicionMontos.FindAsync(id);
            if (condicionMonto == null)
            {
                return NotFound();
            }
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionMonto.CodCondicion);
            return View(condicionMonto);
        }

        // POST: CondicionMonto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodCondicion,Monto")] CondicionMonto condicionMonto)
        {
            if (id != condicionMonto.CodCondicion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(condicionMonto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CondicionMontoExists(condicionMonto.CodCondicion))
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
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionMonto.CodCondicion);
            return View(condicionMonto);
        }

        // GET: CondicionMonto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionMonto = await _context.CondicionMontos
                .Include(c => c.CodCondicionNavigation)
                .FirstOrDefaultAsync(m => m.CodCondicion == id);
            if (condicionMonto == null)
            {
                return NotFound();
            }

            return View(condicionMonto);
        }

        // POST: CondicionMonto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condicionMonto = await _context.CondicionMontos.FindAsync(id);
            if (condicionMonto != null)
            {
                _context.CondicionMontos.Remove(condicionMonto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CondicionMontoExists(int id)
        {
            return _context.CondicionMontos.Any(e => e.CodCondicion == id);
        }
    }
}
