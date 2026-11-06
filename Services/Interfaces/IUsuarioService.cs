using Plataforma24API.DTOs.Usuarios;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Consulta y administracion de usuarios.</summary>
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ListarAsync();

        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no existe.</exception>
        Task<UsuarioDTO> BuscarAsync(int id);

        Task<IEnumerable<UsuarioDTO>> ListarPorCursoAsync(int idCurso);

        /// <summary>
        /// Modifica un usuario. Cada usuario edita su nombre; solo un administrador puede cambiar
        /// el email, el rol y el curso.
        /// </summary>
        /// <exception cref="Common.Exceptions.ForbiddenException">La cuenta es de otro usuario o se intento cambiar el rol o el curso sin ser administrador.</exception>
        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no existe.</exception>
        Task ActualizarAsync(int id, ModificarUsuarioDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no existe.</exception>
        Task EliminarAsync(int id);

        /// <summary>
        /// Borra la contrasena (hash vacio) para que el usuario cree una nueva con su email y su legajo. Las cuentas
        /// administrativas solo las blanquea el Admin general.
        /// </summary>
        /// <exception cref="Common.Exceptions.BadRequestException">Es la propia cuenta.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">La cuenta es administrativa y quien blanquea no es Admin.</exception>
        /// <exception cref="Common.Exceptions.NotFoundException">El usuario no existe.</exception>
        Task BlanquearPasswordAsync(int id);
    }
}
