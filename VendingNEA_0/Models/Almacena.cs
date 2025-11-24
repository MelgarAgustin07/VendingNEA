using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Almacena
{
    public int NumSerie { get; set; }

    public int CodProducto { get; set; }

    public int? Stock { get; set; }

    public virtual Producto CodProductoNavigation { get; set; } = null!;

    public virtual Maquina NumSerieNavigation { get; set; } = null!;
}
