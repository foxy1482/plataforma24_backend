using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Material
{
    public class CrearMaterialDTO
    {
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int NumMateria { get; set; }
    }
}
