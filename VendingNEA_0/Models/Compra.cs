using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Compra
{
    public int CodCompra { get; set; }

    public string Cuitproveedor { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public decimal? MontoTotal { get; set; }

    public virtual Proveedor CuitproveedorNavigation { get; set; } = null!;

    public virtual ICollection<Incluye> Incluyes { get; set; } = new List<Incluye>();
}
