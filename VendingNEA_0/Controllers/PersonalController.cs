using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingNEA_0.Models;
using VendingNEA_0.ViewModel;

namespace VendingNEA_0.Controllers
{
    public class PersonalController : Controller
    {
        private readonly VendingContext _context;

        public PersonalController(VendingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Legajo,Dni,Nombre,Apellido,Direccion,Email,Puesto,FechaIngreso")] Empleado empleado,string? LicenciaConducir,string? Titulo)
        {
            if (!ModelState.IsValid)
                return View(empleado);

            // 1️⃣ Registrar empleado normal
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync(); // aquí se genera el Legajo automáticamente

            // 2️⃣ Dependiendo del puesto, guardar en la tabla correspondiente
            if (empleado.Puesto == "Repositor")
            {
                var repo = new EmpleadoRepositor
                {
                    Legajo = empleado.Legajo,
                    LicenciaConducir = LicenciaConducir
                };

                _context.EmpleadoRepositors.Add(repo);
            }
            else if (empleado.Puesto == "Tecnico")
            {
                var tecnico = new EmpleadoTecnico
                {
                    Legajo = empleado.Legajo,
                    Titulo = Titulo
                };

                _context.EmpleadoTecnicos.Add(tecnico);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Tasks(int? legajo)
        {
            if (legajo == null)
                return NotFound();

            // Buscar empleado con info básica
            var empleado = await _context.Empleados
                .Include(e => e.EmpleadoRepositor)
                .Include(e => e.EmpleadoTecnico)
                .FirstOrDefaultAsync(e => e.Legajo == legajo);

            if (empleado == null)
                return NotFound();

            var vm = new EmpleadoTareasViewModel
            {
                Empleado = empleado
            };

            // Si es repositor → cargar rutas y visitas
            if (empleado.Puesto == "Repositor" && empleado.EmpleadoRepositor != null)
            {
                vm.Rutas = await _context.Ruta
                    .Where(r => r.LegajoRepositor == empleado.Legajo)
                    .Include(r => r.Visita)
                    .ToListAsync();

                vm.Visitas = await _context.Visita
                    .Where(v => v.LegajoRepositor == empleado.Legajo)
                    .Include(v => v.NumSerieMaquinaNavigation)
                    .ToListAsync();
            }

            // Si es técnico → cargar mantenimientos
            if (empleado.Puesto == "Tecnico" && empleado.EmpleadoTecnico != null)
            {
                vm.Mantenimientos = await _context.Mantenimientos
                    .Where(m => m.LegajoTecnico == empleado.Legajo)
                    .Include(m => m.NumSerieMaquinaNavigation)
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}
