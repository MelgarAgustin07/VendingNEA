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
        // Se llama con ?numAcuerdo=123 desde AcuerdosController
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
            [Bind("CodCondicion,Nombre")] Condicion condicion,
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

            // 2) Validaciones básicas
            if (string.IsNullOrWhiteSpace(condicion.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

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

            // 3) Guardar la condición base
            _context.Condicions.Add(condicion);
            await _context.SaveChangesAsync(); // genera CodCondicion

            // 4) Guardar el detalle según tipo
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

            // 5) Asociar condición al acuerdo (muchos-a-muchos)
            acuerdo.CodCondicions.Add(condicion);

            await _context.SaveChangesAsync();

            // 6) Redirigir: podés seguir agregando más condiciones al mismo acuerdo
            return RedirectToAction(nameof(Create), new { numAcuerdo = NumAcuerdo });
        }

        // =========================================================
        // Opcional: listado simple de condiciones
        // =========================================================
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Condicions
                .Include(c => c.CondicionComision)
                .Include(c => c.CondicionMonto)
                .ToListAsync();

            return View(lista);
        }

        // =========================================================
        // Opcional: detalles de una condición
        // =========================================================
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
