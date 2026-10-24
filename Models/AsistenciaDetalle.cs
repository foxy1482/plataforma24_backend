using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("asistencia_detalle")]
    public class AsistenciaDetalle
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_asistencia")]
        public int IdAsistencia { get; set; }

        [Column("id_alumno")]
        public int IdAlumno { get; set; }

        [Column("estado")]
        public EstadoAsistencia Estado { get; set; } = EstadoAsistencia.presente;

        [Column("observacion")]
        public string? Observacion { get; set; }
    }
}