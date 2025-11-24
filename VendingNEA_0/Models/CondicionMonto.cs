using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class CondicionMonto
{
    public int CodCondicion { get; set; }

    public decimal? Monto { get; set; }

    public virtual Condicion CodCondicionNavigation { get; set; } = null!;
}
