using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Condicion
{
    public int CodCondicion { get; set; }

    public string? Nombre { get; set; }

    // 1–1 con CondicionComision
    [ValidateNever]
    public virtual CondicionComision? CondicionComision { get; set; }

    [ValidateNever]
    public virtual CondicionMonto? CondicionMonto { get; set; }

    [ValidateNever]
    public virtual ICollection<Acuerdo> NumAcuerdos { get; set; } = new List<Acuerdo>();

    // public virtual ICollection<Contiene> Contienes { get; set; } = new List<Contiene>();
}
