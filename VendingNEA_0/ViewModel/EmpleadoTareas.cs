using VendingNEA_0.Models;

namespace VendingNEA_0.ViewModel
{
    public class EmpleadoTareasViewModel
    {
        public Empleado Empleado { get; set; }
        public List<Rutum> Rutas { get; set; } = new();
        public List<Visitum> Visitas { get; set; } = new();
        public List<Mantenimiento> Mantenimientos { get; set; } = new();
    }

}
