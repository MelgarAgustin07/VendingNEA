using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Pago
{
    public int NumLiquidacion { get; set; }

    public int NumPago { get; set; }

    public decimal? Monto { get; set; }

    public string? FormaPago { get; set; }

    public DateTime? FechaPago { get; set; }

    public virtual Liquidacion NumLiquidacionNavigation { get; set; } = null!;
}
