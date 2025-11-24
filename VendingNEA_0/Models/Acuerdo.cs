using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Acuerdo
{
    public int NumAcuerdo { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public int NumSerie { get; set; }

    public string? Cuit { get; set; }

    // Navegaciones: no se validan desde el form
    [ValidateNever]
    public virtual Establecimiento? CuitNavigation { get; set; }

    [ValidateNever]
    public virtual Maquina NumSerieNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Liquidacion> Liquidacions { get; set; } = new List<Liquidacion>();

    // Muchos-a-muchos con Condicion (usa la tabla Contiene por atrás)
    [ValidateNever]
    public virtual ICollection<Condicion> CodCondicions { get; set; } = new List<Condicion>();
}
