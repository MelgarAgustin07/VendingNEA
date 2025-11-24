using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models;

public partial class Producto
{
    public int CodProducto { get; set; }

    public string? Nombre { get; set; }

    public string? Categoria { get; set; }

    public decimal? PrecioVenta { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public virtual ICollection<Almacena> Almacenas { get; set; } = new List<Almacena>();

    public virtual ICollection<Incluye> Incluyes { get; set; } = new List<Incluye>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
