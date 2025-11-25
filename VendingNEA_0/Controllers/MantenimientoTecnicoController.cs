using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VendingNEA_0.Models;

namespace VendingNEA_0.Controllers
{
    public class MantenimientoTecnicoController : Controller
    {
        private readonly VendingContext _context;

        public MantenimientoTecnicoController(VendingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string estadoFiltro,
            string tipoFiltro,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var mantenimientos = _context.Mantenimientos
                .Include(m => m.LegajoTecnicoNavigation)
                    .ThenInclude(t => t.LegajoNavigation)
                .Include(m => m.NumSerieMaquinaNavigation)
                    .ThenInclude(ma => ma.MaquinaEfectivo)
                .Include(m => m.NumSerieMaquinaNavigation)
                    .ThenInclude(ma => ma.MaquinaDebito)
                .AsQueryable();

            // Filtrar por estado según la fecha
            if (!string.IsNullOrEmpty(estadoFiltro))
            {
                if (estadoFiltro == "Realizado")
                    mantenimientos = mantenimientos.Where(m => m.Fecha.HasValue && m.Fecha.Value.Date <= DateTime.Today);
                else if (estadoFiltro == "Programado")
                    mantenimientos = mantenimientos.Where(m => m.Fecha.HasValue && m.Fecha.Value.Date > DateTime.Today);
            }

            // Filtrar por tipo de mantenimiento
            if (!string.IsNullOrEmpty(tipoFiltro))
            {
                mantenimientos = mantenimientos.Where(m => m.Tipo == tipoFiltro);
            }

            // Filtrar por rango de fechas
            if (fechaInicio.HasValue)
            {
                mantenimientos = mantenimientos.Where(m => m.Fecha.HasValue && m.Fecha.Value.Date >= fechaInicio.Value.Date);
            }
            if (fechaFin.HasValue)
            {
                mantenimientos = mantenimientos.Where(m => m.Fecha.HasValue && m.Fecha.Value.Date <= fechaFin.Value.Date);
            }

            mantenimientos = mantenimientos.OrderByDescending(m => m.Fecha);

            // Dropdowns para filtros
            ViewBag.EstadoList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Todos", Value = "", Selected = string.IsNullOrEmpty(estadoFiltro) },
                new SelectListItem { Text = "Realizado", Value = "Realizado", Selected = estadoFiltro == "Realizado" },
                new SelectListItem { Text = "Programado", Value = "Programado", Selected = estadoFiltro == "Programado" }
            };

                ViewBag.TipoList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Todos", Value = "", Selected = string.IsNullOrEmpty(tipoFiltro) },
                new SelectListItem { Text = "Preventivo", Value = "Preventivo", Selected = tipoFiltro == "Preventivo" },
                new SelectListItem { Text = "Correctivo", Value = "Correctivo", Selected = tipoFiltro == "Correctivo" }
            };

            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            var lista = await mantenimientos.ToListAsync();
            return View(lista);
        }

        // GET: Mantenimiento/Create
        public IActionResult Create()
        {
            // Técnicos: mostrar "Nombre Apellido"
            var tecnicos = _context.EmpleadoTecnicos
                .Include(e => e.LegajoNavigation)
                .Select(e => new
                {
                    e.Legajo,
                    NombreCompleto = e.LegajoNavigation.Nombre + " " + e.LegajoNavigation.Apellido
                })
                .ToList();

            ViewBag.Tecnicos = new SelectList(tecnicos, "Legajo", "NombreCompleto");

            // Máquinas: mostrar "Marca - Descripcion"
            var maquinas = _context.Maquinas
                .Where(m => !m.IsDeleted)
                .Select(m => new
                {
                    m.NumSerie,
                    Detalle = m.Marca + " - " + m.Descripcion
                })
                .ToList();

            ViewBag.Maquinas = new SelectList(maquinas, "NumSerie", "Detalle");

            return View();
        }

        // POST: Mantenimiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tipo,Costo,Fecha,Descripcion,LegajoTecnico,NumSerieMaquina")] Mantenimiento mantenimiento)
        {
            ModelState.Remove("LegajoTecnicoNavigation");
            ModelState.Remove("NumSerieMaquinaNavigation");
            ModelState.Remove("Incidentes");

            if (ModelState.IsValid)
            {
                _context.Add(mantenimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si falla, repoblar dropdowns con el mismo formato
            var tecnicos = _context.EmpleadoTecnicos
                .Include(e => e.LegajoNavigation)
                .Select(e => new
                {
                    e.Legajo,
                    NombreCompleto = e.LegajoNavigation.Nombre + " " + e.LegajoNavigation.Apellido
                })
                .ToList();

            ViewBag.Tecnicos = new SelectList(tecnicos, "Legajo", "NombreCompleto", mantenimiento.LegajoTecnico);

            var maquinas = _context.Maquinas
                .Select(m => new
                {
                    m.NumSerie,
                    Detalle = m.Marca + " - " + m.Descripcion
                })
                .ToList();

            ViewBag.Maquinas = new SelectList(maquinas, "NumSerie", "Detalle", mantenimiento.NumSerieMaquina);

            return View(mantenimiento);
        }
    }
}
