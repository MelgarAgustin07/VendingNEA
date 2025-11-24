using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Ventum
{
    public int NumOperacion { get; set; }

    public int NumSerie { get; set; }

    public int CodProducto { get; set; }

    public decimal? PrecioTotal { get; set; }

    public DateTime? FechaHora { get; set; }

    public int? Cantidad { get; set; }

    public virtual Producto CodProductoNavigation { get; set; } = null!;

    public virtual Maquina NumSerieNavigation { get; set; } = null!;
}
