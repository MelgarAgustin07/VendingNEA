using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class MaquinaDebito
{
    public int NumSerie { get; set; }

    public virtual Maquina NumSerieNavigation { get; set; } = null!;
}
