using Plataforma24API.DTOs.Entregas;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Entregas de tareas realizadas por los alumnos.</summary>
    public interface IEntregaService
    {
        /// <summary>El personal ve las entregas de cualquier alumno; un alumno, solo las suyas.</summary>
        /// <exception cref="Common.Exceptions.ForbiddenException">Un alumno consulto las entregas de otro.</exception>
        Task<IEnumerable<EntregaRespuestaDTO>> ListarPorAlumnoAsync(int idUsuario);

        Task<IEnumerable<EntregaRespuestaDTO>> ListarPorTareaAsync(int idTarea);

        /// <summary>Lista las entregas de un alumno para las tareas de una materia.</summary>
        /// <exception cref="Common.Exceptions.ForbiddenException">Un alumno consulto las entregas de otro.</exception>
        Task<IEnumerable<EntregaRespuestaDTO>> ListarPorMateriaYAlumnoAsync(int idMateria, int idUsuario);

        /// <summary>Registra la entrega del usuario autenticado para una tarea.</summary>
        /// <exception cref="Common.Exceptions.BadRequestException">La tarea no existe o ya fue entregada.</exception>
        Task<EntregaRespuestaDTO> RealizarAsync(EntregaDTO dto);

        /// <summary>
        /// El autor (o un administrador) puede cambiar la fecha de la entrega; la calificacion
        /// la asignan solo los profesores y administradores.
        /// </summary>
        /// <exception cref="Common.Exceptions.NotFoundException">La entrega no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">Se intento cambiar la fecha de una entrega ajena o calificar sin ser profesor ni administrador.</exception>
        Task ModificarAsync(int id, ModificarEntregaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La entrega no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">La entrega es de otro usuario.</exception>
        Task EliminarAsync(int id);
    }
}
