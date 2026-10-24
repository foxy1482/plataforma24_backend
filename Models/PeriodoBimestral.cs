using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("periodos_bimestrales")]
    public class PeriodoBimestral
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ciclo_lectivo")]
        public int CicloLectivo { get; set; }

        [Column("bimestre")]
        public short Bimestre { get; set; }

        [Column("fecha_inicio", TypeName = "date")]
        public DateOnly FechaInicio { get; set; }

        [Column("fecha_fin", TypeName = "date")]
        public DateOnly FechaFin { get; set; }
    }
}