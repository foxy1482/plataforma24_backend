using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Mappers;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Consulta y edicion de datos personales. Cada usuario accede a los suyos; un administrador, a todos.</summary>
    public class DatosPersonalesService : IDatosPersonalesService
    {
        private readonly IDatosPersonalesRepository _datos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DatosPersonalesService(
            IDatosPersonalesRepository datos,
            IUsuarioRepository usuarios,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _datos = datos;
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<DatosPersonalesDTO> ObtenerPorUsuarioAsync(int idUsuario)
        {
            if (!_currentUser.EsPersonal && !_currentUser.TienePermiso(idUsuario)) throw new ForbiddenException();

            var datos = await _datos.GetByUsuarioIdAsync(idUsuario)
                ?? throw new NotFoundException("El usuario no tiene datos personales cargados.");

            return datos.ToDTO();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DatosPersonalesDTO>> ListarPorCursoAsync(int idCurso)
        {
            var lista = await _datos.ListarPorCursoAsync(idCurso);
            return lista.Select(d => d.ToDTO());
        }

        /// <inheritdoc />
        public async Task<DatosPersonalesDTO> CrearAsync(DatosPersonalesDTO dto)
        {
            if (!_currentUser.TienePermiso(dto.IdUsuario)) throw new ForbiddenException();

            if (!await _usuarios.ExisteAsync(dto.IdUsuario))
                throw new BadRequestException("El usuario especificado no existe.");

            if (await _datos.ExistePorUsuarioAsync(dto.IdUsuario))
                throw new BadRequestException("Ya existen datos personales para este usuario.");

            if (await _datos.ExistePorDniAsync(dto.DNI))
                throw new BadRequestException("Ya existe una persona registrada con ese DNI.");

            var datos = dto.ToEntity();

            await _datos.AddAsync(datos);
            await _unitOfWork.SaveChangesAsync();

            return datos.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int idUsuario, ModificarDatosPersonalesDTO dto)
        {
            if (!_currentUser.TienePermiso(idUsuario)) throw new ForbiddenException();

            var datos = await _datos.GetByUsuarioIdAsync(idUsuario)
                ?? throw new NotFoundException("El usuario no tiene datos personales cargados.");

            if (dto.Nombre != null) datos.Nombre = dto.Nombre;
            if (dto.Apellido != null) datos.Apellido = dto.Apellido;
            if (dto.DNI.HasValue) datos.DNI = dto.DNI.Value;
            if (dto.TelefonoCelular.HasValue) datos.TelefonoCelular = dto.TelefonoCelular;
            if (dto.Direccion != null) datos.Direccion = dto.Direccion;
            if (dto.FechaNacimiento.HasValue) datos.FechaNacimiento = dto.FechaNacimiento.Value;
            if (dto.IdFotoArchivo.HasValue) datos.IdFotoArchivo = dto.IdFotoArchivo;
            if (dto.Sexo.HasValue) datos.Sexo = dto.Sexo;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int idUsuario)
        {
            var datos = await _datos.GetByUsuarioIdAsync(idUsuario)
                ?? throw new NotFoundException("El usuario no tiene datos personales cargados.");

            _datos.Remove(datos);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
