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
    public class AcuerdosController : Controller
    {
        private readonly VendingContext _context;

        public AcuerdosController(VendingContext context)
        {
            _context = context;
        }

        // GET: Acuerdoes
        public async Task<IActionResult> Index()
        {
            var vendingContext = _context.Acuerdos.Include(a => a.CuitNavigation).Include(a => a.NumSerieNavigation);
            return View(await vendingContext.ToListAsync());
        }

        // GET: Acuerdoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acuerdo = await _context.Acuerdos
                .Include(a => a.CuitNavigation)
                .Include(a => a.NumSerieNavigation)
                .FirstOrDefaultAsync(m => m.NumAcuerdo == id);
            if (acuerdo == null)
            {
                return NotFound();
            }

            return View(acuerdo);
        }

        // GET: Acuerdos/Create
        // GET: Acuerdos/Create
        public IActionResult Create(int? numSerie)
        {
            // Si viene desde Details de una máquina
            if (numSerie != null)
            {
                var tieneVigente = _context.Acuerdos
                    .Any(a => a.NumSerie == numSerie && a.FechaFinalizacion >= DateTime.Now);

                if (tieneVigente)
                {
                    TempData["Error"] = "Esta máquina ya tiene un acuerdo vigente.";
                    return RedirectToAction("Details", "Maquinas", new { id = numSerie });
                }

                // Sólo esa máquina
                ViewData["NumSerie"] = new SelectList(
                    _context.Maquinas.Where(m => m.NumSerie == numSerie),
                    "NumSerie",
                    "Descripcion",
                    numSerie
                );

                ViewData["IsFixedMachine"] = true;
            }
            else
            {
                // Modo normal: sólo máquinas sin acuerdo vigente
                var maquinasDisponibles =
                    _context.Maquinas.Where(m =>
                        !_context.Acuerdos.Any(a =>
                            a.NumSerie == m.NumSerie &&
                            a.FechaFinalizacion >= DateTime.Now));

                ViewData["NumSerie"] = new SelectList(
                    maquinasDisponibles,
                    "NumSerie",
                    "Descripcion"
                );

                ViewData["IsFixedMachine"] = false;
            }

            // Establecimientos
            ViewData["Cuit"] = new SelectList(
                _context.Establecimientos,
                "Cuit",
                "NombreComercial"
            );

            // 🔥 Lista de condiciones existentes
            ViewBag.Condiciones = _context.Condicions.ToList();

            return View();
        }

        // POST: Acuerdos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("NumAcuerdo,FechaInicio,FechaFinalizacion,NumSerie,Cuit")] Acuerdo acuerdo,
            int[] CondicionesSeleccionadas)
        {
            // 1) VALIDAR fechas vacías
            if (acuerdo.FechaInicio == null)
                ModelState.AddModelError("FechaInicio", "La fecha de inicio es obligatoria.");

            if (acuerdo.FechaFinalizacion == null)
                ModelState.AddModelError("FechaFinalizacion", "La fecha de finalización es obligatoria.");

            if (!ModelState.IsValid)
            {
                RellenarCombos(acuerdo);
                ViewBag.Condiciones = _context.Condicions.ToList();
                return View(acuerdo);
            }

            // 2) VALIDAR que fin > inicio
            if (acuerdo.FechaFinalizacion < acuerdo.FechaInicio)
            {
                ModelState.AddModelError("", "La fecha de finalización no puede ser anterior a la fecha de inicio.");

                RellenarCombos(acuerdo);
                ViewBag.Condiciones = _context.Condicions.ToList();
                return View(acuerdo);
            }

            // 3) VALIDAR que no haya acuerdo vigente para esa máquina
            var existeVigente = await _context.Acuerdos
                .AnyAsync(a =>
                    a.NumSerie == acuerdo.NumSerie &&
                    a.FechaFinalizacion >= DateTime.Now);

            if (existeVigente)
            {
                ModelState.AddModelError("", "Esta máquina ya tiene un acuerdo vigente.");

                RellenarCombos(acuerdo);
                ViewBag.Condiciones = _context.Condicions.ToList();
                return View(acuerdo);
            }

            // 4) Guardar acuerdo
            _context.Acuerdos.Add(acuerdo);
            await _context.SaveChangesAsync();

            // 5) Asociar condiciones seleccionadas (muchos-a-muchos)
            if (CondicionesSeleccionadas != null && CondicionesSeleccionadas.Length > 0)
            {
                var condiciones = await _context.Condicions
                    .Where(c => CondicionesSeleccionadas.Contains(c.CodCondicion))
                    .ToListAsync();

                foreach (var cond in condiciones)
                {
                    acuerdo.CodCondicions.Add(cond);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 👇 Helper privado para no repetir código
        private void RellenarCombos(Acuerdo acuerdo)
        {
            // OJO: si querés respetar la lógica de máquina fija / disponible
            ViewData["NumSerie"] = new SelectList(
                _context.Maquinas,
                "NumSerie",
                "Descripcion",
                acuerdo.NumSerie
            );

            ViewData["Cuit"] = new SelectList(
                _context.Establecimientos,
                "Cuit",
                "NombreComercial",
                acuerdo.Cuit
            );
        }


        // GET: Acuerdoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acuerdo = await _context.Acuerdos.FindAsync(id);
            if (acuerdo == null)
            {
                return NotFound();
            }
            ViewData["Cuit"] = new SelectList(_context.Establecimientos, "Cuit", "Cuit", acuerdo.Cuit);
            ViewData["NumSerie"] = new SelectList(_context.Maquinas, "NumSerie", "NumSerie", acuerdo.NumSerie);
            return View(acuerdo);
        }

        // POST: Acuerdoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumAcuerdo,FechaInicio,FechaFinalizacion,NumSerie,Cuit")] Acuerdo acuerdo)
        {
            if (id != acuerdo.NumAcuerdo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acuerdo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcuerdoExists(acuerdo.NumAcuerdo))
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
            ViewData["Cuit"] = new SelectList(_context.Establecimientos, "Cuit", "Cuit", acuerdo.Cuit);
            ViewData["NumSerie"] = new SelectList(_context.Maquinas, "NumSerie", "NumSerie", acuerdo.NumSerie);
            return View(acuerdo);
        }

        // GET: Acuerdoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acuerdo = await _context.Acuerdos
                .Include(a => a.CuitNavigation)
                .Include(a => a.NumSerieNavigation)
                .FirstOrDefaultAsync(m => m.NumAcuerdo == id);
            if (acuerdo == null)
            {
                return NotFound();
            }

            return View(acuerdo);
        }

        // POST: Acuerdoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acuerdo = await _context.Acuerdos.FindAsync(id);
            if (acuerdo != null)
            {
                _context.Acuerdos.Remove(acuerdo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcuerdoExists(int id)
        {
            return _context.Acuerdos.Any(e => e.NumAcuerdo == id);
        }
    }
}
