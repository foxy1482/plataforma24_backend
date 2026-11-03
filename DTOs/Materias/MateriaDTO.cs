namespace Plataforma24API.DTOs.Materias
{
    /// <summary>Materia dictada en un curso.</summary>
    public class MateriaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public int? IdProfesor { get; set; }

        public int IdCurso { get; set; }
    }
}
