using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Responsable
{
    public string Dni { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? Email { get; set; }

    public string? Cuitestablecimiento { get; set; }

    public virtual Establecimiento? CuitestablecimientoNavigation { get; set; }
}
