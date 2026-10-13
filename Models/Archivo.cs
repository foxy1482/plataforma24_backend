using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Plataforma24API.Models
{
    [Table("archivos")]
    public class Archivo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_original")]
        public string NombreOriginal { get; set; } = null!;

        [Column("guid_archivo")]
        public Guid GuidArchivo { get; set; }

        [Column("ruta")]
        public string Ruta { get; set; } = null!;

        [Column("tipo")]
        public string Tipo { get; set; } = null!;

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_subida")]
        public DateTime FechaSubida { get; set; }
    }
}