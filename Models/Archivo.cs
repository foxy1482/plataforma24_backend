using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Plataforma24API.Models
{
    [Table("archivos")]
    public class Archivo
    {
        [Key]
        [Column("numero")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Numero { get; set; }

        [Column("nombre_original")]
        public string NombreOriginal { get; set; }

        [Column("guid_archivo")]
        public Guid GuidArchivo { get; set; }

        [Column("ruta")]
        public string Ruta { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("num_legajo")]
        public int NumLegajo { get; set; }

        [Column("fecha_subida")]
        public DateTime FechaSubida { get; set; }
    }
}
