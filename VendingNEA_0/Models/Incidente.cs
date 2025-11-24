using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Incidente
{
    public int NumIncidente { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaReporte { get; set; }

    public string? Estado { get; set; }

    public string? TipoFalla { get; set; }

    public int LegajoEmpleado { get; set; }

    public int NumSerieMaquina { get; set; }

    public int? CodMantenimiento { get; set; }

    public int? CodVisita { get; set; }

    public virtual Mantenimiento? CodMantenimientoNavigation { get; set; }

    public virtual Visitum? CodVisitaNavigation { get; set; }

    public virtual Empleado LegajoEmpleadoNavigation { get; set; } = null!;

    public virtual Maquina NumSerieMaquinaNavigation { get; set; } = null!;
}
