using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Nota de una materia dentro de un boletin.</summary>
    [Table("boletin_notas")]
    public class BoletinNota
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_boletin")]
        public int IdBoletin { get; set; }

        [Column("id_materia")]
        public int IdMateria { get; set; }

        [Column("nota")]
        public decimal? Nota { get; set; }

        [Column("observacion")]
        public string? Observacion { get; set; }

        [Column("id_cargado_por")]
        public int? IdCargadoPor { get; set; }

        [Column("fecha_carga")]
        public DateTime? FechaCarga { get; set; }
    }
}
