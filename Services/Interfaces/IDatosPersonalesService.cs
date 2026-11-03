using Plataforma24API.DTOs.Usuarios;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Datos personales de los usuarios. Cada usuario accede a los suyos; un administrador, a todos.</summary>
    public interface IDatosPersonalesService
    {
        /// <exception cref="Common.Exceptions.ForbiddenException">Los datos pertenecen a otro usuario.</exception>
        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no cargo sus datos.</exception>
        Task<DatosPersonalesDTO> ObtenerPorUsuarioAsync(int idUsuario);

        /// <summary>Lista los datos personales de los usuarios de un curso.</summary>
        Task<IEnumerable<DatosPersonalesDTO>> ListarPorCursoAsync(int idCurso);

        /// <exception cref="Common.Exceptions.ForbiddenException">Los datos son de otro usuario.</exception>
        /// <exception cref="Common.Exceptions.BadRequestException">El usuario no existe, ya tiene datos o el DNI esta registrado.</exception>
        Task<DatosPersonalesDTO> CrearAsync(DatosPersonalesDTO dto);

        /// <exception cref="Common.Exceptions.ForbiddenException">Los datos pertenecen a otro usuario.</exception>
        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no cargo sus datos.</exception>
        Task ActualizarAsync(int idUsuario, ModificarDatosPersonalesDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no cargo sus datos.</exception>
        Task EliminarAsync(int idUsuario);
    }
}
