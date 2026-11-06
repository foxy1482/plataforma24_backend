using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Mappers;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.Services
{
    /// <summary>Consulta y administracion de usuarios. Un usuario edita su propia cuenta; solo un administrador cambia el email, el rol o el curso.</summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IRolRepository _roles;
        private readonly ICursoRepository _cursos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public UsuarioService(
            IUsuarioRepository usuarios,
            IRolRepository roles,
            ICursoRepository cursos,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _usuarios = usuarios;
            _roles = roles;
            _cursos = cursos;
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
            // El preceptor no administra cuentas, pero asigna alumnos a un curso: puede cambiar SOLO el curso.
            var preceptorAsignaCurso = _currentUser.EsPreceptor
                && dto.IdCurso.HasValue
                && dto.IdRol == null
                && dto.Email == null
                && dto.NombreUsuario == null;

            if (!_currentUser.TienePermiso(id) && !preceptorAsignaCurso) throw new ForbiddenException();

            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            if (!_currentUser.EsAdmin && (dto.IdRol.HasValue || dto.Email != null || (dto.IdCurso.HasValue && !_currentUser.EsPreceptor)))
                throw new ForbiddenException("No tenés permisos para modificar el email, el rol o el curso.");

            if (dto.IdCurso.HasValue)
            {
                if (await _cursos.GetByIdAsync(dto.IdCurso.Value) == null)
                    throw new BadRequestException("El curso indicado no existe.");

                usuario.IdCurso = dto.IdCurso.Value;
            }

            if (dto.NombreUsuario != null)
            {
                var nombre = dto.NombreUsuario.Trim();
                if (nombre.Length < 3 || nombre.Length > 50)
                    throw new BadRequestException("El nombre de usuario debe tener entre 3 y 50 caracteres.");

                if (!string.Equals(nombre, usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase)
                    && await _usuarios.ExistePorNombreUsuarioAsync(nombre))
                    throw new BadRequestException("El nombre de usuario ya está en uso.");

                usuario.NombreUsuario = nombre;
            }

            if (_currentUser.EsAdmin)
            {
                if (dto.IdRol.HasValue && dto.IdRol.Value != usuario.IdRol)
                {
                    // Un administrador que se quita su propio rol se queda sin acceso al panel.
                    if (usuario.Id == _currentUser.Id)
                        throw new BadRequestException("No podés cambiar tu propio rol.");

                    if (await _roles.GetByIdAsync(dto.IdRol.Value) == null)
                        throw new BadRequestException("El rol indicado no existe.");

                    // El legajo no se toca acá: el trigger de la base lo renueva (nuevo prefijo, siguiente número libre)
                    // cuando el rol nuevo tiene otro prefijo, de forma atómica con este mismo UPDATE.
                    usuario.IdRol = dto.IdRol.Value;
                }

                if (dto.Email != null)
                {
                    var email = dto.Email.Trim();
                    if (!new EmailAddressAttribute().IsValid(email))
                        throw new BadRequestException("El correo electrónico no es válido.");

                    if (!string.Equals(email, usuario.Email, StringComparison.OrdinalIgnoreCase)
                        && await _usuarios.ExistePorEmailAsync(email))
                        throw new BadRequestException("Ya existe un usuario con ese correo electrónico.");

                    usuario.Email = email;
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task BlanquearPasswordAsync(int id)
        {
            if (id == _currentUser.Id)
                throw new BadRequestException("No podés blanquear tu propia contraseña: cambiala desde tu perfil.");

            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            // Quien blanquea conoce el email y el legajo de la cuenta: si una Autoridad pudiera blanquear a un
            // administrador, podría tomar su cuenta. Las cuentas administrativas solo las blanquea el Admin.
            if (RolIds.EsAdmin(usuario.IdRol) && _currentUser.IdRol != RolIds.Admin)
                throw new ForbiddenException("Solo el administrador general puede blanquear la contraseña de una cuenta administrativa.");

            usuario.PasswordHash = string.Empty;
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            if (id == _currentUser.Id)
                throw new BadRequestException("No podés eliminar tu propia cuenta.");

            var usuario = await _usuarios.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario {id}.");

            _usuarios.Remove(usuario);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
