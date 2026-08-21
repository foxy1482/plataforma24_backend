using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Rol
{
    public class RolDTO
    {
        [Key]
        public int Numero { get; set; }

        public string Nombre { get; set; }
    }
}
