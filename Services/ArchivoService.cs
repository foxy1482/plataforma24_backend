using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Archivos;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Subida, descarga y administracion de archivos. Si el registro en la base falla despues de guardar el archivo fisico, este se elimina para no dejar huerfanos.</summary>
    public class ArchivoService : IArchivoService
    {
        private readonly IArchivoRepository _archivos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IFileStorage _storage;
        private readonly ILogger<ArchivoService> _logger;

        public ArchivoService(
            IArchivoRepository archivos,
            IUsuarioRepository usuarios,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IFileStorage storage,
            ILogger<ArchivoService> logger)
        {
            _archivos = archivos;
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _storage = storage;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ArchivoDTO>> ListarAsync(string? nombre, int? idUsuario)
        {
            var archivos = await _archivos.ListarAsync(nombre, idUsuario);
            return archivos.Select(a => a.ToDTO());
        }

        /// <inheritdoc />
        public async Task<ArchivoDTO> BuscarAsync(int id)
        {
            var archivo = await _archivos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el archivo {id}.");

            return archivo.ToDTO();
        }

        /// <inheritdoc />
        public async Task<ArchivoDTO> SubirAsync(ArchivoParaSubir archivo)
        {
            if (archivo.Longitud == 0)
                throw new BadRequestException("Archivo no seleccionado.");

            int idUsuario = _currentUser.Id;

            var guid = Guid.NewGuid();
            var nombreSeguro = NombreSeguro(archivo.NombreOriginal);
            var nombreUnico = $"{guid}_{nombreSeguro}";

            var ruta = await _storage.GuardarAsync(archivo.Contenido, archivo.Subcarpeta, nombreUnico);

            var entidad = new Archivo
            {
                NombreOriginal = nombreSeguro,
                GuidArchivo = guid,
                Ruta = ruta,
                Tipo = archivo.Tipo,
                FechaSubida = DateTime.UtcNow,
                IdUsuario = idUsuario
            };

            try
            {
                await _archivos.AddAsync(entidad);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _storage.Eliminar(ruta);
                throw;
            }

            return entidad.ToDTO();
        }

        /// <inheritdoc />
        public async Task<ArchivoContenido> ObtenerContenidoAsync(int id)
        {
            var archivo = await _archivos.GetByIdAsync(id)
                ?? throw new NotFoundException("El archivo no existe en la base de datos.");

            var propietario = await _usuarios.GetByIdAsync(archivo.IdUsuario)
                ?? throw new NotFoundException("Propietario del archivo no encontrado.");

            if (!await PuedeAccederAlArchivoAsync(archivo, propietario))
                throw new ForbiddenException("No tenés permiso para ver este archivo.");

            if (!_storage.Existe(archivo.Ruta))
                throw new NotFoundException("El archivo físico fue eliminado o movido.");

            return new ArchivoContenido
            {
                Contenido = _storage.Abrir(archivo.Ruta),
                Tipo = archivo.Tipo,
                NombreOriginal = archivo.NombreOriginal
            };
        }

        /// <inheritdoc />
        public async Task ActualizarMetadatosAsync(int id, ModificarArchivoDTO dto)
        {
            var archivo = await _archivos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el archivo {id}.");

            if (!string.IsNullOrEmpty(dto.NombreOriginal)) archivo.NombreOriginal = dto.NombreOriginal;
            if (dto.IdUsuario.HasValue) archivo.IdUsuario = dto.IdUsuario.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var archivo = await _archivos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el archivo {id}.");

            _archivos.Remove(archivo);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                _storage.Eliminar(archivo.Ruta);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "No se pudo borrar el archivo físico {Ruta}", archivo.Ruta);
            }
        }

        private async Task<bool> PuedeAccederAlArchivoAsync(Archivo archivo, Usuario propietario)
        {
            if (_currentUser.EsAdmin) return true;

            if (archivo.IdUsuario == _currentUser.Id) return true;

            if (_currentUser.EsProfesor && propietario.IdRol == RolIds.Alumno) return true;

            return await _archivos.EstaAdjuntoATareaOMaterialAsync(archivo.Id);
        }

        private static string NombreSeguro(string nombre)
        {
            var soloNombre = Path.GetFileName(nombre);

            foreach (var caracter in Path.GetInvalidFileNameChars())
            {
                soloNombre = soloNombre.Replace(caracter, '_');
            }

            return string.IsNullOrWhiteSpace(soloNombre) ? "archivo" : soloNombre;
        }
    }
}
