using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Liquidacion
{
    public int NumLiquidacion { get; set; }

    public DateTime? FechaEmision { get; set; }

    public decimal? MontoTotal { get; set; }

    public string Cuitestablecimiento { get; set; } = null!;

    public int NumAcuerdo { get; set; }

    public virtual Establecimiento CuitestablecimientoNavigation { get; set; } = null!;

    public virtual Acuerdo NumAcuerdoNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
