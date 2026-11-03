namespace Plataforma24API.DTOs.Materiales
{
    /// <summary>Datos para publicar un material didactico.</summary>
    public class CrearMaterialDTO
    {
        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int IdMateria { get; set; }
    }
}
