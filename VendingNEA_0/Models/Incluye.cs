using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Incluye
{
    public int CodProducto { get; set; }

    public int CodCompra { get; set; }

    public int? Cantidad { get; set; }

    public virtual Compra CodCompraNavigation { get; set; } = null!;

    public virtual Producto CodProductoNavigation { get; set; } = null!;
}
