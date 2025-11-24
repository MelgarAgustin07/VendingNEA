using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Establecimiento
{
    public string Cuit { get; set; } = null!;

    public string? NombreComercial { get; set; }

    public string? UbicacionInterna { get; set; }

    public string? TipoLugar { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<Acuerdo> Acuerdos { get; set; } = new List<Acuerdo>();

    public virtual ICollection<Liquidacion> Liquidacions { get; set; } = new List<Liquidacion>();

    public virtual ICollection<Responsable> Responsables { get; set; } = new List<Responsable>();
}
