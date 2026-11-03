using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Periodos;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de periodos bimestrales.</summary>
    public class PeriodoService : IPeriodoService
    {
        private readonly IPeriodoRepository _periodos;
        private readonly IUnitOfWork _unitOfWork;

        public PeriodoService(IPeriodoRepository periodos, IUnitOfWork unitOfWork)
        {
            _periodos = periodos;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PeriodoDTO>> ListarAsync(int? ciclo)
        {
            var periodos = await _periodos.ListarAsync(ciclo);
            return periodos.Select(p => p.ToDTO());
        }

        /// <inheritdoc />
        public async Task<PeriodoDTO> BuscarAsync(int id)
        {
            var periodo = await _periodos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el período {id}.");

            return periodo.ToDTO();
        }

        /// <inheritdoc />
        public async Task<PeriodoDTO> CrearAsync(PeriodoDTO dto)
        {
            if (dto.Bimestre < 1 || dto.Bimestre > 4)
                throw new BadRequestException("El bimestre debe ser un número entre 1 y 4.");

            if (dto.FechaFin <= dto.FechaInicio)
                throw new BadRequestException("La fecha de fin debe ser posterior a la de inicio.");

            if (await _periodos.ExisteAsync(dto.CicloLectivo, dto.Bimestre))
                throw new BadRequestException("Ya existe ese bimestre para el ciclo lectivo.");

            var periodo = new PeriodoBimestral
            {
                CicloLectivo = dto.CicloLectivo,
                Bimestre = dto.Bimestre,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin
            };

            await _periodos.AddAsync(periodo);
            await _unitOfWork.SaveChangesAsync();

            return periodo.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarPeriodoDTO dto)
        {
            var periodo = await _periodos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el período {id}.");

            if (dto.FechaInicio.HasValue) periodo.FechaInicio = dto.FechaInicio.Value;
            if (dto.FechaFin.HasValue) periodo.FechaFin = dto.FechaFin.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var periodo = await _periodos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el período {id}.");
            _periodos.Remove(periodo);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
