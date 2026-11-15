using Microsoft.EntityFrameworkCore;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Infrastructure.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();
    public DbSet<Tratamiento> Tratamientos => Set<Tratamiento>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<DetalleReceta> DetallesReceta => Set<DetalleReceta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Especialidad>(entity =>
        {
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Licencia).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.Licencia).IsUnique();
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Cedula).IsUnique();
            entity.Property(e => e.FechaNacimiento).HasColumnType("date");
            entity.Property(e => e.Sexo).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.Property(e => e.Motivo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.Property(e => e.Sintomas).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
        });

        modelBuilder.Entity<Diagnostico>(entity =>
        {
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(300);
        });

        modelBuilder.Entity<Tratamiento>(entity =>
        {
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(300);
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Presentacion).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Concentracion).HasMaxLength(50);
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.Property(e => e.Indicaciones).HasMaxLength(500);
        });

        modelBuilder.Entity<DetalleReceta>(entity =>
        {
            entity.Property(e => e.Dosis).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Frecuencia).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Medico>()
            .HasOne<Especialidad>()
            .WithMany()
            .HasForeignKey(e => e.EspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cita>()
            .HasOne<Paciente>()
            .WithMany()
            .HasForeignKey(e => e.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cita>()
            .HasOne<Medico>()
            .WithMany()
            .HasForeignKey(e => e.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Consulta>()
            .HasOne<Cita>()
            .WithMany()
            .HasForeignKey(e => e.CitaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Diagnostico>()
            .HasOne<Consulta>()
            .WithMany()
            .HasForeignKey(e => e.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tratamiento>()
            .HasOne<Diagnostico>()
            .WithMany()
            .HasForeignKey(e => e.DiagnosticoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Receta>()
            .HasOne<Consulta>()
            .WithMany()
            .HasForeignKey(e => e.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DetalleReceta>()
            .HasOne<Receta>()
            .WithMany()
            .HasForeignKey(e => e.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DetalleReceta>()
            .HasOne<Medicamento>()
            .WithMany()
            .HasForeignKey(e => e.MedicamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Especialidad>().HasData(
            new Especialidad { Id = 1, Nombre = "Medicina General", Descripcion = "Atención médica general" },
            new Especialidad { Id = 2, Nombre = "Pediatría", Descripcion = "Atención médica para niños" },
            new Especialidad { Id = 3, Nombre = "Cardiología", Descripcion = "Enfermedades del corazón" }
        );

        modelBuilder.Entity<Medico>().HasData(
            new Medico { Id = 1, Nombre = "Carlos", Apellido = "Méndez", Licencia = "LIC-001", Telefono = "809-555-0101", Email = "carlos.mendez@clinica.com", EspecialidadId = 1 },
            new Medico { Id = 2, Nombre = "Laura", Apellido = "Peña", Licencia = "LIC-002", Telefono = "809-555-0102", Email = "laura.pena@clinica.com", EspecialidadId = 2 }
        );

        modelBuilder.Entity<Paciente>().HasData(
            new Paciente { Id = 1, Nombre = "María", Apellido = "López", Cedula = "001-1234567-8", FechaNacimiento = new DateTime(1990, 5, 12), Sexo = "Femenino", Telefono = "809-555-0201", Email = "maria.lopez@correo.com", Direccion = "Santo Domingo" },
            new Paciente { Id = 2, Nombre = "José", Apellido = "Ramírez", Cedula = "001-7654321-0", FechaNacimiento = new DateTime(1985, 11, 3), Sexo = "Masculino", Telefono = "809-555-0202", Email = "jose.ramirez@correo.com", Direccion = "Santiago" }
        );

        modelBuilder.Entity<Medicamento>().HasData(
            new Medicamento { Id = 1, Nombre = "Paracetamol", Presentacion = "Tabletas", Concentracion = "500 mg" },
            new Medicamento { Id = 2, Nombre = "Amoxicilina", Presentacion = "Cápsulas", Concentracion = "500 mg" }
        );
    }
}
