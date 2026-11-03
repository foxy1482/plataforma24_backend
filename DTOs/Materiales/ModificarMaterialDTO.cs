using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Materiales
{
    /// <summary>Cambios sobre un material. Los campos nulos no se modifican.</summary>
    public class ModificarMaterialDTO
    {
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }
    }
}
