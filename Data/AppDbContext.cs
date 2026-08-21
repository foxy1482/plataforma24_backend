using Microsoft.EntityFrameworkCore;
using Plataforma24API.Models;

namespace Plataforma24API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<DatosPersonales> DatosPersonales { get; set; }

        public DbSet<Rol> Roles { get; set; }

        public DbSet<Curso> Cursos { get; set; }

        public DbSet<Materia> Materias { get; set; }

        public DbSet<Tarea> Tareas { get; set; }

        public DbSet<Archivo> Archivos { get; set; }

        public DbSet<Material> Materiales { get; set; }

        public DbSet<Entrega> Entregas { get; set; }

        public DbSet<TareaArchivo> TareaArchivos { get; set; }

        public DbSet<MaterialArchivo> MaterialArchivos { get; set; }

        public DbSet<EntregaArchivo> EntregaArchivos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.HasPostgresEnum<Sexo>();
            modelBuilder.HasPostgresEnum<Turno>();
            modelBuilder.HasPostgresEnum<Division>();
            //modelBuilder.Entity<Curso>(entity =>
            //{
            //    entity.Property(e => e.Año)
            //        .HasColumnType("years") // <--- ESTO ES LA CLAVE
            //        .HasConversion(
            //            v => v.ToString(),
            //            v => int.Parse(v)
            //        );
            //});
        }
    }
}
