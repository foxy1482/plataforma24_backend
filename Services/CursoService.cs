using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Cursos;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de cursos.</summary>
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursos;
        private readonly IUnitOfWork _unitOfWork;

        public CursoService(ICursoRepository cursos, IUnitOfWork unitOfWork)
        {
            _cursos = cursos;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CursoDTO>> ListarAsync(Year? ano, Turno? turno)
        {
            var cursos = await _cursos.ListarAsync(ano, turno);
            return cursos.Select(c => c.ToDTO());
        }

        /// <inheritdoc />
        public async Task<CursoDTO> BuscarAsync(int id)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            return curso.ToDTO();
        }

        /// <inheritdoc />
        public async Task<CursoDTO> CrearAsync(CursoDTO dto)
        {
            if (await _cursos.ExisteAsync(dto.Año, dto.Division, dto.Turno, dto.CicloLectivo))
                throw new BadRequestException("Ya existe un curso con ese año, división, turno y ciclo lectivo.");

            var curso = new Curso
            {
                Año = dto.Año,
                Division = dto.Division,
                Turno = dto.Turno,
                Aula = dto.Aula,
                CicloLectivo = dto.CicloLectivo
            };

            await _cursos.AddAsync(curso);
            await _unitOfWork.SaveChangesAsync();

            return curso.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarCursoDTO dto)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            if (dto.Año.HasValue) curso.Año = dto.Año.Value;
            if (dto.Division.HasValue) curso.Division = dto.Division.Value;
            if (dto.Turno.HasValue) curso.Turno = dto.Turno.Value;
            if (dto.Aula != null) curso.Aula = dto.Aula;
            if (dto.CicloLectivo.HasValue) curso.CicloLectivo = dto.CicloLectivo.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            _cursos.Remove(curso);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
