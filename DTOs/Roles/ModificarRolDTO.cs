using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Roles
{
    /// <summary>Cambios sobre un rol. Los campos nulos no se modifican.</summary>
    public class ModificarRolDTO
    {
        public string? Nombre { get; set; }
    }
}
