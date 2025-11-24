using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Empleado
{
    public int Legajo { get; set; }

    public int? Dni { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Direccion { get; set; }

    public string? Email { get; set; }

    public string? Puesto { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public virtual EmpleadoRepositor? EmpleadoRepositor { get; set; }

    public virtual EmpleadoTecnico? EmpleadoTecnico { get; set; }

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
