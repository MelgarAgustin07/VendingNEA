using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Visitum
{
    public int CodVisita { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime? HoraInicio { get; set; }

    public DateTime? HoraFinalizacion { get; set; }

    public string? Observaciones { get; set; }

    public int LegajoRepositor { get; set; }

    public int NumSerieMaquina { get; set; }

    public int? CodRuta { get; set; }

    public virtual Rutum? CodRutaNavigation { get; set; }

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual EmpleadoRepositor LegajoRepositorNavigation { get; set; } = null!;

    public virtual Maquina NumSerieMaquinaNavigation { get; set; } = null!;
}
