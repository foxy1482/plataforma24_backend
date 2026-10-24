using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public DbSet<PeriodoBimestral> PeriodosBimestrales { get; set; }

        public DbSet<Boletin> Boletines { get; set; }

        public DbSet<BoletinNota> BoletinNotas { get; set; }

        public DbSet<Asistencia> Asistencias { get; set; }

        public DbSet<AsistenciaDetalle> AsistenciaDetalles { get; set; }

        public DbSet<PalabraProhibida> PalabrasProhibidas { get; set; }

        public DbSet<ForoTema> ForoTemas { get; set; }

        public DbSet<ForoMensaje> ForoMensajes { get; set; }

        public DbSet<ForoReporte> ForoReportes { get; set; }

        public DbSet<VBoletinResumen> VBoletinResumenes { get; set; }

        public DbSet<VBoletinDetalle> VBoletinDetalles { get; set; }

        public DbSet<VInasistenciaPeriodo> VInasistenciasPeriodo { get; set; }

        public DbSet<VInasistenciaAnual> VInasistenciasAnuales { get; set; }

        public DbSet<VForoTema> VForoTemas { get; set; }

        public DbSet<VForoMensaje> VForoMensajes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Sexo>();
            modelBuilder.HasPostgresEnum<Turno>();
            modelBuilder.HasPostgresEnum<Division>();
            modelBuilder.HasPostgresEnum<EstadoBoletin>();
            modelBuilder.HasPostgresEnum<EstadoAsistencia>();
            modelBuilder.HasPostgresEnum<ModoCoincidencia>();

            modelBuilder.Entity<Usuario>(e =>
            {
                e.Property(u => u.NumeroLegajo).ValueGeneratedOnAdd();
                e.Property(u => u.NumeroLegajo).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(u => u.FechaAlta).HasDefaultValueSql("LOCALTIMESTAMP");
            });

            modelBuilder.Entity<Boletin>().Property(b => b.FechaCreacion).HasDefaultValueSql("LOCALTIMESTAMP");
            modelBuilder.Entity<Asistencia>().Property(a => a.FechaRegistro).HasDefaultValueSql("LOCALTIMESTAMP");
            modelBuilder.Entity<PalabraProhibida>().Property(p => p.FechaAlta).HasDefaultValueSql("LOCALTIMESTAMP");
            modelBuilder.Entity<ForoTema>().Property(t => t.FechaCreacion).HasDefaultValueSql("LOCALTIMESTAMP");
            modelBuilder.Entity<ForoMensaje>().Property(m => m.FechaPublicacion).HasDefaultValueSql("LOCALTIMESTAMP");
            modelBuilder.Entity<ForoReporte>().Property(r => r.Fecha).HasDefaultValueSql("LOCALTIMESTAMP");

            modelBuilder.Entity<VBoletinResumen>().HasNoKey().ToView("v_boletin_resumen");
            modelBuilder.Entity<VBoletinDetalle>().HasNoKey().ToView("v_boletin_detalle");
            modelBuilder.Entity<VInasistenciaPeriodo>().HasNoKey().ToView("v_inasistencias_periodo");
            modelBuilder.Entity<VInasistenciaAnual>().HasNoKey().ToView("v_inasistencias_anuales");
            modelBuilder.Entity<VForoTema>().HasNoKey().ToView("v_foro_temas");
            modelBuilder.Entity<VForoMensaje>().HasNoKey().ToView("v_foro_mensajes");
        }
    }
}