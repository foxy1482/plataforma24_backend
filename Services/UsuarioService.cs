using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Mappers;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Consulta y administracion de usuarios. Un usuario edita su propia cuenta; solo un administrador cambia el email, el rol o el curso.</summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public UsuarioService(IUsuarioRepository usuarios, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UsuarioDTO>> ListarAsync()
        {
            var usuarios = await _usuarios.GetAllAsync();
            return usuarios.Select(u => u.ToDTO());
        }

        /// <inheritdoc />
        public async Task<UsuarioDTO> BuscarAsync(int id)
        {
            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            return usuario.ToDTO();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UsuarioDTO>> ListarPorCursoAsync(int idCurso)
        {
            var usuarios = await _usuarios.ListarPorCursoAsync(idCurso);
            return usuarios.Select(u => u.ToDTO());
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarUsuarioDTO dto)
        {
            if (!_currentUser.TienePermiso(id)) throw new ForbiddenException();

            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            if (!_currentUser.EsAdmin && (dto.IdRol.HasValue || dto.IdCurso.HasValue))
                throw new ForbiddenException("No tenés permisos para modificar el rol o el curso.");

            if (dto.NombreUsuario != null) usuario.NombreUsuario = dto.NombreUsuario;

            if (_currentUser.EsAdmin)
            {
                if (dto.IdRol.HasValue) usuario.IdRol = dto.IdRol.Value;
                if (dto.IdCurso.HasValue) usuario.IdCurso = dto.IdCurso.Value;
                if (dto.Email != null) usuario.Email = dto.Email;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            _usuarios.Remove(usuario);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
