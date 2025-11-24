using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Mantenimiento
{
    public int CodMantenimiento { get; set; }

    public string? Tipo { get; set; }

    public decimal? Costo { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Descripcion { get; set; }

    public int LegajoTecnico { get; set; }

    public int NumSerieMaquina { get; set; }

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual EmpleadoTecnico LegajoTecnicoNavigation { get; set; } = null!;

    public virtual Maquina NumSerieMaquinaNavigation { get; set; } = null!;
}
