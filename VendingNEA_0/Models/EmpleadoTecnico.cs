using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class EmpleadoTecnico
{
    public int Legajo { get; set; }

    public string? Titulo { get; set; }

    public virtual Empleado LegajoNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}
