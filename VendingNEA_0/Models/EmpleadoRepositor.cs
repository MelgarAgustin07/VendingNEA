using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class EmpleadoRepositor
{
    public int Legajo { get; set; }

    public string? LicenciaConducir { get; set; }

    public virtual Empleado LegajoNavigation { get; set; } = null!;

    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
