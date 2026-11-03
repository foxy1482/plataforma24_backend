using Plataforma24API.DTOs.Cursos;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Curso"/> a DTO.</summary>
    public static class CursoMapper
    {
        public static CursoDTO ToDTO(this Curso curso)
        {
            return new CursoDTO
            {
                Id = curso.Id,
                Año = curso.Año,
                Division = curso.Division,
                Turno = curso.Turno,
                Aula = curso.Aula,
                CicloLectivo = curso.CicloLectivo
            };
        }
    }
}
