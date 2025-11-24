using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Rutum
{
    public int CodRuta { get; set; }

    public DateTime? Fecha { get; set; }

    public int LegajoRepositor { get; set; }

    public string? PatenteVehiculo { get; set; }

    public virtual EmpleadoRepositor LegajoRepositorNavigation { get; set; } = null!;

    public virtual Vechiculo? PatenteVehiculoNavigation { get; set; }

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
