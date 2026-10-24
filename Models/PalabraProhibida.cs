using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("palabras_prohibidas")]
    public class PalabraProhibida
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("palabra")]
        public string Palabra { get; set; } = null!;

        [Column("modo")]
        public ModoCoincidencia Modo { get; set; } = ModoCoincidencia.palabra_completa;

        [Column("activa")]
        public bool Activa { get; set; } = true;

        [Column("id_agregada_por")]
        public int? IdAgregadaPor { get; set; }

        [Column("fecha_alta")]
        public DateTime FechaAlta { get; set; }
    }
}