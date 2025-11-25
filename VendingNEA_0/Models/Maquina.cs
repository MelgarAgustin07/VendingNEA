using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VendingNEA_0.Models;

public partial class Maquina
{
    public int NumSerie { get; set; }

    [Required(ErrorMessage = "La ubicación es obligatoria.")]
    public string? Ubicacion { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el estado operativo.")]
    public bool? EstadoOperativo { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    public string? Marca { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ICollection<Acuerdo> Acuerdos { get; set; } = new List<Acuerdo>();
    public virtual ICollection<Almacena> Almacenas { get; set; } = new List<Almacena>();
    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
    public virtual MaquinaDebito? MaquinaDebito { get; set; }
    public virtual MaquinaEfectivo? MaquinaEfectivo { get; set; }
    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
