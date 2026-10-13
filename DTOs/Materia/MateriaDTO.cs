namespace Plataforma24API.DTOs.Materia
{
    public class MateriaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public int? IdProfesor { get; set; }

        public int IdCurso { get; set; }
    }
}