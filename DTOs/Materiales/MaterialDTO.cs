namespace Plataforma24API.DTOs.Materiales
{
    /// <summary>Material didactico de una materia.</summary>
    public class MaterialDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int IdMateria { get; set; }
    }
}
