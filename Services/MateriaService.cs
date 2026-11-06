using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Materias;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>
    /// Administracion de materias. Un profesor crea, modifica y elimina sus propias materias y elige el curso;
    /// un administrador gestiona todas y asigna el profesor a cargo.
    /// </summary>
    public class MateriaService : IMateriaService
    {
        private const int LargoMinimoNombre = 3;

        private readonly IMateriaRepository _materias;
        private readonly ICursoRepository _cursos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public MateriaService(
            IMateriaRepository materias,
            ICursoRepository cursos,
            IUsuarioRepository usuarios,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _materias = materias;
            _cursos = cursos;
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MateriaDTO>> ListarAsync(int? idProfesor, int? idCurso)
        {
            var materias = await _materias.ListarAsync(idProfesor, idCurso);
            return materias.Select(m => m.ToDTO());
        }

        /// <inheritdoc />
        public async Task<MateriaDTO> BuscarAsync(int id)
        {
            var materia = await _materias.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia {id}.");

            return materia.ToDTO();
        }

        /// <inheritdoc />
        public async Task<MateriaDTO> CrearAsync(MateriaDTO dto)
        {
            var nombre = NormalizarNombre(dto.Nombre);

            // El docente queda a cargo de lo que crea; solo un administrador asigna a otro. Un docente
            // con permisos administrativos (AdminProfesor) que no elige profesor se la asigna a sí mismo.
            var esDocente = _currentUser.EsProfesor || _currentUser.IdRol == RolIds.AdminProfesor;
            int idProfesor;
            if (_currentUser.EsAdmin && dto.IdProfesor.HasValue && dto.IdProfesor != _currentUser.Id)
            {
                await ValidarProfesorAsync(dto.IdProfesor.Value);
                idProfesor = dto.IdProfesor.Value;
            }
            else if (esDocente)
            {
                idProfesor = _currentUser.Id;
            }
            else if (_currentUser.EsAdmin)
            {
                throw new BadRequestException("Elegí el profesor a cargo de la materia.");
            }
            else
            {
                throw new ForbiddenException("Solo los profesores y los administradores crean materias.");
            }

            await ValidarCursoAsync(dto.IdCurso);

            if (await _materias.ExisteAsync(dto.IdCurso, nombre))
                throw new BadRequestException("Ya existe una materia con ese nombre en el curso.");

            var materia = new Materia
            {
                Nombre = nombre,
                IdProfesor = idProfesor,
                IdCurso = dto.IdCurso
            };

            await _materias.AddAsync(materia);
            await _unitOfWork.SaveChangesAsync();

            return materia.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarMateriaDTO dto)
        {
            var materia = await _materias.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia {id}.");

            ExigirPermisoSobre(materia);

            if (dto.IdProfesor.HasValue && dto.IdProfesor != materia.IdProfesor)
            {
                if (!_currentUser.EsAdmin)
                    throw new ForbiddenException("Solo un administrador puede cambiar el profesor a cargo.");

                await ValidarProfesorAsync(dto.IdProfesor.Value);
            }

            var nombre = dto.Nombre != null ? NormalizarNombre(dto.Nombre) : materia.Nombre;
            var idCurso = dto.IdCurso ?? materia.IdCurso;

            if (idCurso != materia.IdCurso) await ValidarCursoAsync(idCurso);

            var cambioIdentidad = nombre != materia.Nombre || idCurso != materia.IdCurso;
            if (cambioIdentidad && await _materias.ExisteAsync(idCurso, nombre))
                throw new BadRequestException("Ya existe una materia con ese nombre en el curso.");

            materia.Nombre = nombre;
            materia.IdCurso = idCurso;
            if (dto.IdProfesor.HasValue) materia.IdProfesor = dto.IdProfesor.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var materia = await _materias.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia {id}.");

            ExigirPermisoSobre(materia);

            _materias.Remove(materia);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>Un administrador toca cualquier materia; un profesor, solo las que tiene a cargo.</summary>
        private void ExigirPermisoSobre(Materia materia)
        {
            if (_currentUser.EsAdmin) return;

            if (_currentUser.EsProfesor && materia.IdProfesor == _currentUser.Id) return;

            throw new ForbiddenException("Solo podés modificar las materias que tenés a cargo.");
        }

        private static string NormalizarNombre(string? nombre)
        {
            var limpio = (nombre ?? string.Empty).Trim();

            if (limpio.Length < LargoMinimoNombre)
                throw new BadRequestException($"El nombre de la materia debe tener al menos {LargoMinimoNombre} caracteres.");

            if (limpio.Length > 80)
                throw new BadRequestException("El nombre de la materia no puede superar los 80 caracteres.");

            return limpio;
        }

        private async Task ValidarCursoAsync(int idCurso)
        {
            if (await _cursos.GetByIdAsync(idCurso) == null)
                throw new BadRequestException("El curso elegido no existe.");
        }

        private async Task ValidarProfesorAsync(int idProfesor)
        {
            var profesor = await _usuarios.GetByIdAsync(idProfesor)
                ?? throw new BadRequestException("El profesor elegido no existe.");

            if (!RolIds.EsProfesor(profesor.IdRol) && profesor.IdRol != RolIds.AdminProfesor)
                throw new BadRequestException("El usuario elegido no es profesor.");
        }
    }
}
