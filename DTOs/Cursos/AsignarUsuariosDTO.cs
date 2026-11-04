namespace Plataforma24API.DTOs.Cursos
{
    /// <summary>Alumnos a asignar a un curso.</summary>
    public class AsignarUsuariosDTO
    {
        public List<int> IdUsuarios { get; set; } = new();
    }

    /// <summary>Resultado de asignar alumnos a un curso.</summary>
    public class AsignacionCursoDTO
    {
        public int Asignados { get; set; }
    }
}
