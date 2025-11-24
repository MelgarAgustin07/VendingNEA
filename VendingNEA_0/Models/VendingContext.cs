using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VendingNEA_0.Models;

public partial class VendingContext : DbContext
{
    public VendingContext()
    {
    }

    public VendingContext(DbContextOptions<VendingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acuerdo> Acuerdos { get; set; }

    public virtual DbSet<Almacena> Almacenas { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

   // public virtual DbSet<Contiene> Contienes { get; set; }


    public virtual DbSet<Condicion> Condicions { get; set; }

    public virtual DbSet<CondicionComision> CondicionComisions { get; set; }

    public virtual DbSet<CondicionMonto> CondicionMontos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EmpleadoRepositor> EmpleadoRepositors { get; set; }

    public virtual DbSet<EmpleadoTecnico> EmpleadoTecnicos { get; set; }

    public virtual DbSet<Establecimiento> Establecimientos { get; set; }

    public virtual DbSet<Incidente> Incidentes { get; set; }

    public virtual DbSet<Incluye> Incluyes { get; set; }

    public virtual DbSet<Liquidacion> Liquidacions { get; set; }

    public virtual DbSet<Mantenimiento> Mantenimientos { get; set; }

    public virtual DbSet<Maquina> Maquinas { get; set; }

    public virtual DbSet<MaquinaDebito> MaquinaDebitos { get; set; }

    public virtual DbSet<MaquinaEfectivo> MaquinaEfectivos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Responsable> Responsables { get; set; }

    public virtual DbSet<Rutum> Ruta { get; set; }

    public virtual DbSet<Vechiculo> Vechiculos { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    public virtual DbSet<Visitum> Visita { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=VendingConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acuerdo>(entity =>
        {
            entity.HasKey(e => e.NumAcuerdo).HasName("PK__Acuerdo__92F183B6C5DD101B");

            entity.ToTable("Acuerdo");

            entity.Property(e => e.Cuit)
                .HasMaxLength(11)
                .HasColumnName("CUIT");

            entity.HasOne(d => d.CuitNavigation).WithMany(p => p.Acuerdos)
                .HasForeignKey(d => d.Cuit)
                .HasConstraintName("FK_Acuerdo_Establecimiento");

            entity.HasOne(d => d.NumSerieNavigation).WithMany(p => p.Acuerdos)
                .HasForeignKey(d => d.NumSerie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Acuerdo_Maquina");
        });

        modelBuilder.Entity<Almacena>(entity =>
        {
            entity.HasKey(e => new { e.NumSerie, e.CodProducto });

            entity.ToTable("Almacena");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Almacenas)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Almacena_Producto");

            entity.HasOne(d => d.NumSerieNavigation).WithMany(p => p.Almacenas)
                .HasForeignKey(d => d.NumSerie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Almacena_Maquina");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.CodCompra).HasName("PK__Compra__47045A2FA860145F");

            entity.ToTable("Compra");

            entity.Property(e => e.Cuitproveedor)
                .HasMaxLength(11)
                .HasColumnName("CUITProveedor");
            entity.Property(e => e.MontoTotal).HasColumnType("money");

            entity.HasOne(d => d.CuitproveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.Cuitproveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Proveedor");
        });

      //  modelBuilder.Entity<Contiene>(entity =>
      //  {
      //      entity.HasKey(e => new { e.CodCondicion, e.NumAcuerdo });
      //
      //      entity.ToTable("Contiene");
      //
      //      entity.Property(e => e.CodCondicion).HasColumnName("CodCondicion");
      //      entity.Property(e => e.NumAcuerdo).HasColumnName("NumAcuerdo");
      //
      //      entity.HasOne(d => d.CodCondicionNavigation)
      //          .WithMany(p => p.Contienes)
      //          .HasForeignKey(d => d.CodCondicion)
      //          .OnDelete(DeleteBehavior.ClientSetNull);
      //
      //      entity.HasOne(d => d.NumAcuerdoNavigation)
      //          .WithMany(p => p.Contienes)
      //          .HasForeignKey(d => d.NumAcuerdo)
      //          .OnDelete(DeleteBehavior.ClientSetNull);
      //  });


        modelBuilder.Entity<Condicion>(entity =>
        {
            entity.HasKey(e => e.CodCondicion).HasName("PK__Condicio__310BA6718DA75E4B");

            entity.ToTable("Condicion");

            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasMany(d => d.NumAcuerdos).WithMany(p => p.CodCondicions)
                .UsingEntity<Dictionary<string, object>>(
                    "Contiene",
                    r => r.HasOne<Acuerdo>().WithMany()
                        .HasForeignKey("NumAcuerdo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Contiene_Acuerdo"),
                    l => l.HasOne<Condicion>().WithMany()
                        .HasForeignKey("CodCondicion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Contiene_Condicion"),
                    j =>
                    {
                        j.HasKey("CodCondicion", "NumAcuerdo");
                        j.ToTable("Contiene");
                    });
        });

        modelBuilder.Entity<CondicionComision>(entity =>
        {
            entity.HasKey(e => e.CodCondicion).HasName("PK__Condicio__310BA67177C964A7");

            entity.ToTable("CondicionComision");

            entity.Property(e => e.CodCondicion).ValueGeneratedNever();

            entity.HasOne(d => d.CodCondicionNavigation).WithOne(p => p.CondicionComision)
                .HasForeignKey<CondicionComision>(d => d.CodCondicion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CondicionComision_Condicion");
        });

        modelBuilder.Entity<CondicionMonto>(entity =>
        {
            entity.HasKey(e => e.CodCondicion).HasName("PK__Condicio__310BA671572C7D03");

            entity.ToTable("CondicionMonto");

            entity.Property(e => e.CodCondicion).ValueGeneratedNever();
            entity.Property(e => e.Monto).HasColumnType("money");

            entity.HasOne(d => d.CodCondicionNavigation).WithOne(p => p.CondicionMonto)
                .HasForeignKey<CondicionMonto>(d => d.CodCondicion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CondicionMonto_Condicion");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Legajo).HasName("PK__Empleado__0E01039B2549E679");

            entity.ToTable("Empleado");

            entity.Property(e => e.Legajo).ValueGeneratedNever();
            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(100);
            entity.Property(e => e.Dni).HasColumnName("DNI");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Puesto).HasMaxLength(100);
        });

        modelBuilder.Entity<EmpleadoRepositor>(entity =>
        {
            entity.HasKey(e => e.Legajo).HasName("PK__Empleado__0E01039B07472E3A");

            entity.ToTable("EmpleadoRepositor");

            entity.Property(e => e.Legajo).ValueGeneratedNever();
            entity.Property(e => e.LicenciaConducir).HasMaxLength(100);

            entity.HasOne(d => d.LegajoNavigation).WithOne(p => p.EmpleadoRepositor)
                .HasForeignKey<EmpleadoRepositor>(d => d.Legajo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoRepositor_Empleado");
        });

        modelBuilder.Entity<EmpleadoTecnico>(entity =>
        {
            entity.HasKey(e => e.Legajo).HasName("PK__Empleado__0E01039B8A7EDA00");

            entity.ToTable("EmpleadoTecnico");

            entity.Property(e => e.Legajo).ValueGeneratedNever();
            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.LegajoNavigation).WithOne(p => p.EmpleadoTecnico)
                .HasForeignKey<EmpleadoTecnico>(d => d.Legajo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoTecnico_Empleado");
        });

        modelBuilder.Entity<Establecimiento>(entity =>
        {
            entity.HasKey(e => e.Cuit).HasName("PK__Establec__F46C1599B387B1D1");

            entity.ToTable("Establecimiento");

            entity.Property(e => e.Cuit)
                .HasMaxLength(11)
                .HasColumnName("CUIT");
            entity.Property(e => e.Direccion).HasMaxLength(100);
            entity.Property(e => e.NombreComercial).HasMaxLength(100);
            entity.Property(e => e.TipoLugar).HasMaxLength(100);
            entity.Property(e => e.UbicacionInterna).HasMaxLength(100);
        });

        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.HasKey(e => e.NumIncidente).HasName("PK__Incident__A14503556335B1B1");

            entity.ToTable("Incidente");

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(100);
            entity.Property(e => e.TipoFalla).HasMaxLength(100);

            entity.HasOne(d => d.CodMantenimientoNavigation).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.CodMantenimiento)
                .HasConstraintName("FK_Incidente_Mantenimiento");

            entity.HasOne(d => d.CodVisitaNavigation).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.CodVisita)
                .HasConstraintName("FK_Incidente_Visita");

            entity.HasOne(d => d.LegajoEmpleadoNavigation).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.LegajoEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Empleado");

            entity.HasOne(d => d.NumSerieMaquinaNavigation).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.NumSerieMaquina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Maquina");
        });

        modelBuilder.Entity<Incluye>(entity =>
        {
            entity.HasKey(e => new { e.CodProducto, e.CodCompra });

            entity.ToTable("Incluye");

            entity.HasOne(d => d.CodCompraNavigation).WithMany(p => p.Incluyes)
                .HasForeignKey(d => d.CodCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incluye_Compra");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Incluyes)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incluye_Producto");
        });

        modelBuilder.Entity<Liquidacion>(entity =>
        {
            entity.HasKey(e => e.NumLiquidacion).HasName("PK__Liquidac__CB96F91A26CC3E59");

            entity.ToTable("Liquidacion");

            entity.Property(e => e.Cuitestablecimiento)
                .HasMaxLength(11)
                .HasColumnName("CUITEstablecimiento");
            entity.Property(e => e.MontoTotal).HasColumnType("money");

            entity.HasOne(d => d.CuitestablecimientoNavigation).WithMany(p => p.Liquidacions)
                .HasForeignKey(d => d.Cuitestablecimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Liquidacion_Establecimiento");

            entity.HasOne(d => d.NumAcuerdoNavigation).WithMany(p => p.Liquidacions)
                .HasForeignKey(d => d.NumAcuerdo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Liquidacion_Acuerdo");
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.HasKey(e => e.CodMantenimiento).HasName("PK__Mantenim__91C6B832FF2CA4E5");

            entity.ToTable("Mantenimiento");

            entity.Property(e => e.Costo).HasColumnType("money");
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Tipo).HasMaxLength(100);

            entity.HasOne(d => d.LegajoTecnicoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.LegajoTecnico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_Tecnico");

            entity.HasOne(d => d.NumSerieMaquinaNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.NumSerieMaquina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_Maquina");
        });

        modelBuilder.Entity<Maquina>(entity =>
        {
            entity.HasKey(e => e.NumSerie).HasName("PK__Maquina__63AD426367B4387B");

            entity.ToTable("Maquina");

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Ubicacion).HasMaxLength(100);
        });

        modelBuilder.Entity<MaquinaDebito>(entity =>
        {
            entity.HasKey(e => e.NumSerie).HasName("PK__MaquinaD__63AD42631509BB4F");

            entity.ToTable("MaquinaDebito");

            entity.Property(e => e.NumSerie).ValueGeneratedNever();

            entity.HasOne(d => d.NumSerieNavigation).WithOne(p => p.MaquinaDebito)
                .HasForeignKey<MaquinaDebito>(d => d.NumSerie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaquinaDebito_Maquina");
        });

        modelBuilder.Entity<MaquinaEfectivo>(entity =>
        {
            entity.HasKey(e => e.NumSerie).HasName("PK__MaquinaE__63AD4263CB373D09");

            entity.ToTable("MaquinaEfectivo");

            entity.Property(e => e.NumSerie).ValueGeneratedNever();
            entity.Property(e => e.DineroAcumulado).HasColumnType("money");

            entity.HasOne(d => d.NumSerieNavigation).WithOne(p => p.MaquinaEfectivo)
                .HasForeignKey<MaquinaEfectivo>(d => d.NumSerie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaquinaEfectivo_Maquina");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => new { e.NumLiquidacion, e.NumPago });

            entity.ToTable("Pago");

            entity.Property(e => e.FormaPago).HasMaxLength(100);
            entity.Property(e => e.Monto).HasColumnType("money");

            entity.HasOne(d => d.NumLiquidacionNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.NumLiquidacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Liquidacion");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.CodProducto).HasName("PK__Producto__0D06FDF313B3AE90");

            entity.ToTable("Producto");

            entity.Property(e => e.Categoria).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PrecioUnitario).HasColumnType("money");
            entity.Property(e => e.PrecioVenta).HasColumnType("money");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Cuit).HasName("PK__Proveedo__F46C1599B084F444");

            entity.ToTable("Proveedor");

            entity.Property(e => e.Cuit)
                .HasMaxLength(11)
                .HasColumnName("CUIT");
            entity.Property(e => e.Direccion).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(100);
        });

        modelBuilder.Entity<Responsable>(entity =>
        {
            entity.HasKey(e => e.Dni).HasName("PK__Responsa__C035B8DC4AE78AA1");

            entity.ToTable("Responsable");

            entity.Property(e => e.Dni)
                .HasMaxLength(100)
                .HasColumnName("DNI");
            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Cuitestablecimiento)
                .HasMaxLength(11)
                .HasColumnName("CUITEstablecimiento");
            entity.Property(e => e.Direccion).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(100);

            entity.HasOne(d => d.CuitestablecimientoNavigation).WithMany(p => p.Responsables)
                .HasForeignKey(d => d.Cuitestablecimiento)
                .HasConstraintName("FK_Responsable_Establecimiento");
        });

        modelBuilder.Entity<Rutum>(entity =>
        {
            entity.HasKey(e => e.CodRuta).HasName("PK__Ruta__6AAAF6BEA23F0FDF");

            entity.Property(e => e.PatenteVehiculo).HasMaxLength(10);

            entity.HasOne(d => d.LegajoRepositorNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.LegajoRepositor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Repositor");

            entity.HasOne(d => d.PatenteVehiculoNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.PatenteVehiculo)
                .HasConstraintName("FK_Ruta_Vehiculo");
        });

        modelBuilder.Entity<Vechiculo>(entity =>
        {
            entity.HasKey(e => e.Patente).HasName("PK__Vechicul__CA655167AB0E60DA");

            entity.ToTable("Vechiculo");

            entity.Property(e => e.Patente).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(100);
            entity.Property(e => e.Modelo).HasMaxLength(100);
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.NumOperacion).HasName("PK__Venta__51428E4D90479FCF");

            entity.Property(e => e.PrecioTotal).HasColumnType("money");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Producto");

            entity.HasOne(d => d.NumSerieNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.NumSerie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Maquina");
        });

        modelBuilder.Entity<Visitum>(entity =>
        {
            entity.HasKey(e => e.CodVisita).HasName("PK__Visita__74DA340071EBD1CE");

            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(d => d.CodRutaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.CodRuta)
                .HasConstraintName("FK_Visita_Ruta");

            entity.HasOne(d => d.LegajoRepositorNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.LegajoRepositor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visita_Repositor");

            entity.HasOne(d => d.NumSerieMaquinaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.NumSerieMaquina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visita_Maquina");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
