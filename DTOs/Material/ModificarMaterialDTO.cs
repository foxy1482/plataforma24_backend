using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Material
{
    public class ModificarMaterialDTO
    {
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }

    }
}
