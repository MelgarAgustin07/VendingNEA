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
    public class MaquinasController : Controller
    {
        private readonly VendingContext _context;

        public MaquinasController(VendingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string estadoFiltro, string ubicacionFiltro)
        {
            var maquinas = _context.Maquinas.AsQueryable();

            // Filtrar por estado operativo
            if (!string.IsNullOrEmpty(estadoFiltro))
            {
                bool estado = estadoFiltro == "true";
                maquinas = maquinas.Where(m => m.EstadoOperativo == estado);
            }

            // Filtrar por ubicación
            if (!string.IsNullOrEmpty(ubicacionFiltro))
            {
                maquinas = maquinas.Where(m => m.Ubicacion.Contains(ubicacionFiltro));
            }

            // Crear lista de opciones para el dropdown
            ViewBag.EstadoList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Todos", Value = "", Selected = string.IsNullOrEmpty(estadoFiltro) },
                new SelectListItem { Text = "Operando", Value = "true", Selected = estadoFiltro == "true" },
                new SelectListItem { Text = "Fuera de Servicio", Value = "false", Selected = estadoFiltro == "false" },
            };

            ViewBag.UbicacionFiltro = ubicacionFiltro;

            var lista = await maquinas.ToListAsync();
            return View(lista);
        }

        // GET: Maquinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Buscamos la máquina
            var maquina = await _context.Maquinas
                 .Include(m => m.MaquinaDebito)
                 .Include(m => m.MaquinaEfectivo)
                .FirstOrDefaultAsync(m => m.NumSerie == id);

            if (maquina == null)
            {
                return NotFound();
            }

            // ===============================
            // BUSCAR ACUERDO VIGENTE
            // ===================
            // ============
            var acuerdoVigente = await _context.Acuerdos
                .Include(a => a.CuitNavigation) // carga datos del establecimiento
                .Where(a => a.NumSerie == id && a.FechaFinalizacion >= DateTime.Now)
                .FirstOrDefaultAsync();

            // Enviarlo a la vista
            ViewBag.AcuerdoVigente = acuerdoVigente;

            return View(maquina);
        }

        // GET: Maquinas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Maquinas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumSerie,Ubicacion,Descripcion,EstadoOperativo,Marca")] Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maquina);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maquina);
        }

        // GET: Maquinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maquina = await _context.Maquinas.FindAsync(id);
            if (maquina == null)
            {
                return NotFound();
            }
            return View(maquina);
        }

        // POST: Maquinas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumSerie,Ubicacion,Descripcion,EstadoOperativo,Marca")] Maquina maquina)
        {
            if (id != maquina.NumSerie)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maquina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaquinaExists(maquina.NumSerie))
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
            return View(maquina);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);

            if (maquina == null)
                return NotFound();

            _context.Maquinas.Remove(maquina);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool MaquinaExists(int id)
        {
            return _context.Maquinas.Any(e => e.NumSerie == id);
        }
    }
}
