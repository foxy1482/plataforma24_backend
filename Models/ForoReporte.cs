using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Reporte de un mensaje del foro, pendiente de revision por moderacion.</summary>
    [Table("foro_reportes")]
    public class ForoReporte
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_mensaje")]
        public int IdMensaje { get; set; }

        [Column("id_reportante")]
        public int IdReportante { get; set; }

        [Column("motivo")]
        public string Motivo { get; set; } = null!;

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("resuelto")]
        public bool Resuelto { get; set; }

        [Column("id_resuelto_por")]
        public int? IdResueltoPor { get; set; }
    }
}
