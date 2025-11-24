using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class CondicionComision
{
    public int CodCondicion { get; set; }

    public int? Porcentaje { get; set; }

    public virtual Condicion CodCondicionNavigation { get; set; } = null!;
}
