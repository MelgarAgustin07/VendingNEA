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
    public class CondicionComisionController : Controller
    {
        private readonly VendingContext _context;

        public CondicionComisionController(VendingContext context)
        {
            _context = context;
        }

        // GET: CondicionComision
        public async Task<IActionResult> Index()
        {
            var vendingContext = _context.CondicionComisions.Include(c => c.CodCondicionNavigation);
            return View(await vendingContext.ToListAsync());
        }

        // GET: CondicionComision/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionComision = await _context.CondicionComisions
                .Include(c => c.CodCondicionNavigation)
                .FirstOrDefaultAsync(m => m.CodCondicion == id);
            if (condicionComision == null)
            {
                return NotFound();
            }

            return View(condicionComision);
        }

        // GET: CondicionComision/Create
        public IActionResult Create()
        {
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion");
            return View();
        }

        // POST: CondicionComision/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodCondicion,Porcentaje")] CondicionComision condicionComision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(condicionComision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionComision.CodCondicion);
            return View(condicionComision);
        }

        // GET: CondicionComision/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionComision = await _context.CondicionComisions.FindAsync(id);
            if (condicionComision == null)
            {
                return NotFound();
            }
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionComision.CodCondicion);
            return View(condicionComision);
        }

        // POST: CondicionComision/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodCondicion,Porcentaje")] CondicionComision condicionComision)
        {
            if (id != condicionComision.CodCondicion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(condicionComision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CondicionComisionExists(condicionComision.CodCondicion))
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
            ViewData["CodCondicion"] = new SelectList(_context.Condicions, "CodCondicion", "CodCondicion", condicionComision.CodCondicion);
            return View(condicionComision);
        }

        // GET: CondicionComision/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionComision = await _context.CondicionComisions
                .Include(c => c.CodCondicionNavigation)
                .FirstOrDefaultAsync(m => m.CodCondicion == id);
            if (condicionComision == null)
            {
                return NotFound();
            }

            return View(condicionComision);
        }

        // POST: CondicionComision/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condicionComision = await _context.CondicionComisions.FindAsync(id);
            if (condicionComision != null)
            {
                _context.CondicionComisions.Remove(condicionComision);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CondicionComisionExists(int id)
        {
            return _context.CondicionComisions.Any(e => e.CodCondicion == id);
        }
    }
}
