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
        // GET: Acuerdos
        public async Task<IActionResult> Index(string orden, string estado)
        {
            ViewData["OrdenActual"] = orden;
            ViewData["EstadoActual"] = estado;

            var hoy = DateTime.Today;

            var acuerdos = _context.Acuerdos
                .Include(a => a.CuitNavigation)
                .Include(a => a.NumSerieNavigation)
                .AsQueryable();

            // ---- FILTRO POR ESTADO DE VIGENCIA ----
            if (!string.IsNullOrEmpty(estado))
            {
                switch (estado)
                {
                    case "vigentes":
                        acuerdos = acuerdos.Where(a => a.FechaFinalizacion >= hoy);
                        break;

                    case "porVencer":
                        acuerdos = acuerdos.Where(a =>
                            a.FechaFinalizacion >= hoy &&
                            a.FechaFinalizacion <= hoy.AddMonths(1));
                        break;

                    case "vencidos":
                        acuerdos = acuerdos.Where(a => a.FechaFinalizacion < hoy);
                        break;
                }
            }

            // ---- ORDEN ----
            switch (orden)
            {
                case "fechaAsc":
                    acuerdos = acuerdos.OrderBy(a => a.FechaFinalizacion);
                    break;

                case "fechaDesc":
                    acuerdos = acuerdos.OrderByDescending(a => a.FechaFinalizacion);
                    break;

                case "estabAsc":
                    acuerdos = acuerdos.OrderBy(a => a.CuitNavigation.NombreComercial);
                    break;

                case "estabDesc":
                    acuerdos = acuerdos.OrderByDescending(a => a.CuitNavigation.NombreComercial);
                    break;

                default:
                    acuerdos = acuerdos.OrderBy(a => a.NumAcuerdo);
                    break;
            }

            return View(await acuerdos.AsNoTracking().ToListAsync());
        }

        // GET: Acuerdoes/Details/5
        // GET: Acuerdos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acuerdo = await _context.Acuerdos
                .Include(a => a.CuitNavigation)
                .Include(a => a.NumSerieNavigation)
                .Include(a => a.CodCondicions)
                    .ThenInclude(c => c.CondicionComision)
                .Include(a => a.CodCondicions)
                    .ThenInclude(c => c.CondicionMonto)
                .FirstOrDefaultAsync(m => m.NumAcuerdo == id);

            if (acuerdo == null)
            {
                return NotFound();
            }

            return View(acuerdo);
        }


        // ================================
        // GET: Acuerdos/Create
        // ================================
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
                // Modo normal: sólo máquinas SIN acuerdo vigente
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

            return View();
        }

        // ================================
        // POST: Acuerdos/Create
        // ================================
        // POST: Acuerdos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("NumAcuerdo,FechaInicio,FechaFinalizacion,NumSerie,Cuit")] Acuerdo acuerdo,
            string TipoCondicion,
            decimal? Porcentaje,
            decimal? Monto)
        {
            // 1) VALIDAR fechas vacías
            if (acuerdo.FechaInicio == null)
                ModelState.AddModelError("FechaInicio", "La fecha de inicio es obligatoria.");

            if (acuerdo.FechaFinalizacion == null)
                ModelState.AddModelError("FechaFinalizacion", "La fecha de finalización es obligatoria.");

            // 2) Validar tipo de condición
            if (string.IsNullOrWhiteSpace(TipoCondicion))
                ModelState.AddModelError("", "Debes seleccionar un tipo de condición.");

            if (TipoCondicion == "comision")
            {
                if (Porcentaje == null)
                {
                    ModelState.AddModelError("", "Debe ingresar un porcentaje.");
                }
                else if (Porcentaje < 0 || Porcentaje > 100)
                {
                    ModelState.AddModelError("", "El porcentaje debe estar entre 0 y 100.");
                }
            }
            else if (TipoCondicion == "monto")
            {
                if (Monto == null)
                {
                    ModelState.AddModelError("", "Debe ingresar un monto.");
                }
                else if (Monto < 0)
                {
                    ModelState.AddModelError("", "El monto no puede ser negativo.");
                }
            }

            // Si ya hay errores hasta acá → volver al formulario
            if (!ModelState.IsValid)
            {
                RellenarCombos(acuerdo);
                return View(acuerdo);
            }

            // 3) VALIDAR que la fecha final > inicio
            if (acuerdo.FechaFinalizacion < acuerdo.FechaInicio)
            {
                ModelState.AddModelError("", "La fecha de finalización no puede ser anterior a la fecha de inicio.");

                RellenarCombos(acuerdo);
                return View(acuerdo);
            }

            // 4) VALIDAR que no haya acuerdo vigente para esa máquina
            var existeVigente = await _context.Acuerdos
                .AnyAsync(a =>
                    a.NumSerie == acuerdo.NumSerie &&
                    a.FechaFinalizacion >= DateTime.Now);

            if (existeVigente)
            {
                ModelState.AddModelError("", "Esta máquina ya tiene un acuerdo vigente.");

                RellenarCombos(acuerdo);
                return View(acuerdo);
            }

            // 5) Guardar el acuerdo
            _context.Acuerdos.Add(acuerdo);
            await _context.SaveChangesAsync();   // acá ya tenemos NumAcuerdo

            // 6) Crear la condición base (nombre automático según tipo)
            var condicion = new Condicion
            {
                Nombre = TipoCondicion == "comision"
                    ? $"Comisión {Porcentaje}%"
                    : $"Monto fijo ${Monto}"
            };

            _context.Condicions.Add(condicion);
            await _context.SaveChangesAsync();   // genera CodCondicion

            // 7) Crear el detalle según el tipo
            if (TipoCondicion == "comision")
            {
                var comision = new CondicionComision
                {
                    CodCondicion = condicion.CodCondicion,
                    CodCondicionNavigation = condicion,
                    Porcentaje = (int)Porcentaje!.Value
                };
                _context.CondicionComisions.Add(comision);
            }
            else if (TipoCondicion == "monto")
            {
                var montoFijo = new CondicionMonto
                {
                    CodCondicion = condicion.CodCondicion,
                    CodCondicionNavigation = condicion,
                    Monto = Monto!.Value
                };
                _context.CondicionMontos.Add(montoFijo);
            }

            // 8) Asociar condición al acuerdo (muchos-a-muchos → tabla Contiene)
            acuerdo.CodCondicions.Add(condicion);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // 👇 Helper privado para no repetir código
        private void RellenarCombos(Acuerdo acuerdo)
        {
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

            // En reintentos dejamos IsFixedMachine = false (simplifica)
            ViewData["IsFixedMachine"] = false;
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
