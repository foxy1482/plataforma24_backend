using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>
    /// Consulta y edicion de datos personales. Cada usuario accede a los suyos; los administradores y los
    /// preceptores cargan, modifican y eliminan los de cualquier usuario; el resto del personal solo los consulta.
    /// </summary>
    public class DatosPersonalesService : IDatosPersonalesService
    {
        private static readonly DateOnly FechaNacimientoMinima = new(1900, 1, 1);

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

        /// <summary>Administradores y preceptores gestionan los datos de cualquiera; los demas, solo los propios.</summary>
        private bool PuedeGestionar(int idUsuario) =>
            _currentUser.EsAdmin || _currentUser.EsPreceptor || _currentUser.Id == idUsuario;

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
            if (!PuedeGestionar(dto.IdUsuario))
                throw new ForbiddenException("No tenés permisos para cargar los datos personales de otro usuario.");

            dto.Nombre = LimpiarObligatorio(dto.Nombre, "El nombre es obligatorio.");
            dto.Apellido = LimpiarObligatorio(dto.Apellido, "El apellido es obligatorio.");
            dto.Direccion = LimpiarObligatorio(dto.Direccion, "El domicilio es obligatorio.");
            ValidarFechaNacimiento(dto.FechaNacimiento);
            ValidarSexo(dto.Sexo);

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
            if (!PuedeGestionar(idUsuario))
                throw new ForbiddenException("No tenés permisos para modificar los datos personales de otro usuario.");

            var datos = await _datos.GetByUsuarioIdAsync(idUsuario)
                ?? throw new NotFoundException("El usuario no tiene datos personales cargados.");

            if (dto.Nombre != null) datos.Nombre = LimpiarObligatorio(dto.Nombre, "El nombre no puede estar vacío.");
            if (dto.Apellido != null) datos.Apellido = LimpiarObligatorio(dto.Apellido, "El apellido no puede estar vacío.");
            if (dto.Direccion != null) datos.Direccion = LimpiarObligatorio(dto.Direccion, "El domicilio no puede estar vacío.");

            if (dto.DNI.HasValue && dto.DNI.Value != datos.DNI)
            {
                if (await _datos.ExistePorDniDeOtroAsync(dto.DNI.Value, idUsuario))
                    throw new BadRequestException("Ya existe una persona registrada con ese DNI.");

                datos.DNI = dto.DNI.Value;
            }

            if (dto.TelefonoCelular.HasValue) datos.TelefonoCelular = dto.TelefonoCelular;

            if (dto.FechaNacimiento.HasValue)
            {
                ValidarFechaNacimiento(dto.FechaNacimiento.Value);
                datos.FechaNacimiento = dto.FechaNacimiento.Value;
            }

            if (dto.IdFotoArchivo.HasValue) datos.IdFotoArchivo = dto.IdFotoArchivo;

            if (dto.Sexo.HasValue)
            {
                ValidarSexo(dto.Sexo);
                datos.Sexo = dto.Sexo;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int idUsuario)
        {
            if (!_currentUser.EsAdmin && !_currentUser.EsPreceptor)
                throw new ForbiddenException("No tenés permisos para eliminar datos personales.");

            var datos = await _datos.GetByUsuarioIdAsync(idUsuario)
                ?? throw new NotFoundException("El usuario no tiene datos personales cargados.");

            _datos.Remove(datos);
            await _unitOfWork.SaveChangesAsync();
        }

        private static string LimpiarObligatorio(string? valor, string mensaje)
        {
            var limpio = valor?.Trim();
            if (string.IsNullOrEmpty(limpio)) throw new BadRequestException(mensaje);
            return limpio;
        }

        private static void ValidarFechaNacimiento(DateOnly fecha)
        {
            if (fecha < FechaNacimientoMinima)
                throw new BadRequestException("La fecha de nacimiento no es válida.");

            if (fecha > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                throw new BadRequestException("La fecha de nacimiento no puede ser futura.");
        }

        private static void ValidarSexo(Sexo? sexo)
        {
            if (sexo.HasValue && !Enum.IsDefined(sexo.Value))
                throw new BadRequestException("El sexo indicado no es válido.");
        }
    }
}
