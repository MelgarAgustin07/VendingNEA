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
            var maquinas = _context.Maquinas
                .Include(m => m.Acuerdos)
                .Where(m => !m.IsDeleted)
                .AsQueryable();

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
                return NotFound();

            var maquina = await _context.Maquinas
                .Include(m => m.MaquinaDebito)
                .Include(m => m.MaquinaEfectivo)
                .FirstOrDefaultAsync(m => m.NumSerie == id && !m.IsDeleted);

            if (maquina == null)
                return NotFound();

            // Buscar acuerdo vigente
            var acuerdoVigente = await _context.Acuerdos
                .Include(a => a.CuitNavigation)
                .Where(a => a.NumSerie == id && a.FechaFinalizacion >= DateTime.Now)
                .FirstOrDefaultAsync();

            ViewBag.AcuerdoVigente = acuerdoVigente;

            return View(maquina);
        }

        // GET: Maquinas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Maquinas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumSerie,Ubicacion,Descripcion,EstadoOperativo,Marca")] Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                maquina.IsDeleted = false;

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
                return NotFound();

            var maquina = await _context.Maquinas
                .Where(m => !m.IsDeleted)
                .FirstOrDefaultAsync(m => m.NumSerie == id);

            if (maquina == null)
                return NotFound();

            return View(maquina);
        }

        // POST: Maquinas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumSerie,Ubicacion,Descripcion,EstadoOperativo,Marca")] Maquina maquina)
        {
            if (id != maquina.NumSerie)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var original = await _context.Maquinas.AsNoTracking().FirstAsync(m => m.NumSerie == id);
                    maquina.IsDeleted = original.IsDeleted;

                    _context.Update(maquina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaquinaExists(maquina.NumSerie))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(maquina);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var maquina = await _context.Maquinas
                .FirstOrDefaultAsync(m => m.NumSerie == id && !m.IsDeleted);

            if (maquina == null)
                return NotFound();

            maquina.IsDeleted = true;
            maquina.EstadoOperativo = false;
            _context.Maquinas.Update(maquina);

            // Buscar acuerdo vigente
            var acuerdo = await _context.Acuerdos
                .Where(a => a.NumSerie == id && a.FechaFinalizacion >= DateTime.Now)
                .FirstOrDefaultAsync();

            if (acuerdo != null)
            {
                acuerdo.FechaFinalizacion = DateTime.Now;
                _context.Acuerdos.Update(acuerdo);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool MaquinaExists(int id)
        {
            return _context.Maquinas.Any(e => e.NumSerie == id);
        }
    }
}
