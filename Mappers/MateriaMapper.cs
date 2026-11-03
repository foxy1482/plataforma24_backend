using Plataforma24API.DTOs.Materias;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Materia"/> a DTO.</summary>
    public static class MateriaMapper
    {
        public static MateriaDTO ToDTO(this Materia materia)
        {
            return new MateriaDTO
            {
                Id = materia.Id,
                Nombre = materia.Nombre,
                IdProfesor = materia.IdProfesor,
                IdCurso = materia.IdCurso
            };
        }
    }
}
