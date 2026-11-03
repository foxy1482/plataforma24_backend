using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Materias;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de materias.</summary>
    public class MateriaService : IMateriaService
    {
        private readonly IMateriaRepository _materias;
        private readonly IUnitOfWork _unitOfWork;

        public MateriaService(IMateriaRepository materias, IUnitOfWork unitOfWork)
        {
            _materias = materias;
            _unitOfWork = unitOfWork;
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
            if (await _materias.ExisteAsync(dto.IdCurso, dto.Nombre))
                throw new BadRequestException("Ya existe una materia con ese nombre en el curso.");

            var materia = new Materia
            {
                Nombre = dto.Nombre,
                IdProfesor = dto.IdProfesor,
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

            if (dto.Nombre != null) materia.Nombre = dto.Nombre;
            if (dto.IdProfesor.HasValue) materia.IdProfesor = dto.IdProfesor.Value;
            if (dto.IdCurso.HasValue) materia.IdCurso = dto.IdCurso.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var materia = await _materias.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia {id}.");

            _materias.Remove(materia);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
