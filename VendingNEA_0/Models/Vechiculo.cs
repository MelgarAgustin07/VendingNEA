using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Vechiculo
{
    public string Patente { get; set; } = null!;

    public int? Capacidad { get; set; }

    public string? Modelo { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaUltMantenimiento { get; set; }

    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();
}
