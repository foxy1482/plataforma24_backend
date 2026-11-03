namespace Plataforma24API.DTOs.Materias
{
    /// <summary>Cambios sobre una materia. Los campos nulos no se modifican.</summary>
    public class ModificarMateriaDTO
    {
        public string? Nombre { get; set; }

        public int? IdProfesor { get; set; }

        public int? IdCurso { get; set; }
    }
}
