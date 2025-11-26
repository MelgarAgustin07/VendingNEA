using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingNEA_0.Models;

namespace VendingNEA_0.Controllers
{
    public class CondicionsController : Controller
    {
        private readonly VendingContext _context;

        public CondicionsController(VendingContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: Condicions/Create
        // Se llama con ?numAcuerdo=123 desde Acuerdos/Details
        // =========================================================
        public async Task<IActionResult> Create(int numAcuerdo)
        {
            var acuerdo = await _context.Acuerdos
                .FirstOrDefaultAsync(a => a.NumAcuerdo == numAcuerdo);

            if (acuerdo == null)
            {
                return NotFound("No se encontró el acuerdo especificado.");
            }

            ViewBag.NumAcuerdo = numAcuerdo;
            return View();
        }

        // =========================================================
        // POST: Condicions/Create
        // Crea la condición, el tipo (comisión/monto) y la asocia al acuerdo
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Condicion condicion,
            int NumAcuerdo,
            string TipoCondicion,
            decimal? Porcentaje,
            decimal? Monto)
        {
            // 1) Buscar el acuerdo
            var acuerdo = await _context.Acuerdos
                .Include(a => a.CodCondicions)
                .FirstOrDefaultAsync(a => a.NumAcuerdo == NumAcuerdo);

            if (acuerdo == null)
            {
                ModelState.AddModelError("", "No se encontró el acuerdo asociado.");
            }

            // 2) Validaciones básicas de tipo
            if (string.IsNullOrWhiteSpace(TipoCondicion))
                ModelState.AddModelError("", "Debes seleccionar un tipo de condición.");

            // Validar según tipo
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

            // Si hay errores → volver a la vista
            if (!ModelState.IsValid)
            {
                ViewBag.NumAcuerdo = NumAcuerdo;
                return View(condicion);
            }

            // 3) Generar automáticamente el Nombre de la condición
            if (TipoCondicion == "comision")
            {
                condicion.Nombre = $"Comisión {Porcentaje}%";
            }
            else if (TipoCondicion == "monto")
            {
                condicion.Nombre = $"Monto fijo ${Monto}";
            }

            // 4) Guardar la condición base
            _context.Condicions.Add(condicion);
            await _context.SaveChangesAsync(); // genera CodCondicion

            // 5) Guardar el detalle según tipo
            if (TipoCondicion == "comision")
            {
                var comision = new CondicionComision
                {
                    CodCondicion = condicion.CodCondicion,
                    Porcentaje = (int)Porcentaje!.Value
                };
                _context.CondicionComisions.Add(comision);
            }
            else if (TipoCondicion == "monto")
            {
                var montoFijo = new CondicionMonto
                {
                    CodCondicion = condicion.CodCondicion,
                    Monto = Monto!.Value
                };
                _context.CondicionMontos.Add(montoFijo);
            }

            // 6) Asociar condición al acuerdo (muchos-a-muchos)
            acuerdo.CodCondicions.Add(condicion);

            await _context.SaveChangesAsync();

            // 7) Volver al detalle del acuerdo
            return RedirectToAction("Details", "Acuerdos", new { id = NumAcuerdo });
        }

        // =========================================================
        // Listado simple de condiciones (opcional)
        // =========================================================
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Condicions
                .Include(c => c.CondicionComision)
                .Include(c => c.CondicionMonto)
                .ToListAsync();

            return View(lista);
        }

        // Detalles de una condición (opcional)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var condicion = await _context.Condicions
                .Include(c => c.CondicionComision)
                .Include(c => c.CondicionMonto)
                .Include(c => c.NumAcuerdos)
                .FirstOrDefaultAsync(c => c.CodCondicion == id);

            if (condicion == null) return NotFound();

            return View(condicion);
        }
    }
}
