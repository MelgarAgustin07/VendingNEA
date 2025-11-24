using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Proveedor
{
    public string Cuit { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Email { get; set; }

    public string? Nombre { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
