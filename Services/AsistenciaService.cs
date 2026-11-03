using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Asistencias;
using Plataforma24API.Mappers;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Apertura, marcado y consulta de planillas de asistencia.</summary>
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _asistencias;
        private readonly IAsistenciaDetalleRepository _detalles;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public AsistenciaService(
            IAsistenciaRepository asistencias,
            IAsistenciaDetalleRepository detalles,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _asistencias = asistencias;
            _detalles = detalles;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<int> AbrirPlanillaAsync(AbrirAsistenciaDTO dto)
        {
            if (dto.Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                throw new BadRequestException("Solo se puede abrir asistencia de lunes a viernes.");

            return await _asistencias.AbrirPlanillaAsync(dto.IdCurso, dto.Fecha, _currentUser.Id);
        }

        /// <inheritdoc />
        public async Task<PlanillaAsistenciaDTO> ObtenerPlanillaAsync(int idCurso, DateOnly? fecha)
        {
            var fechaConsulta = fecha ?? DateOnly.FromDateTime(DateTime.Today);

            var planilla = await _asistencias.GetPorCursoYFechaAsync(idCurso, fechaConsulta)
                ?? throw new NotFoundException("No hay planilla abierta para ese curso y fecha. Usá POST /asistencias/abrir.");

            var filas = await _detalles.ListarConAlumnoAsync(planilla.Id);
            var detalle = filas.Select(f => f.Detalle.ToDTO(f.Alumno, f.Datos)).ToList();

            return planilla.ToPlanillaDTO(detalle);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AsistenciaDTO>> ListarPlanillasAsync(int idCurso, DateOnly? desde, DateOnly? hasta)
        {
            var planillas = await _asistencias.ListarPorCursoAsync(idCurso, desde, hasta);
            return planillas.Select(a => a.ToDTO());
        }

        /// <inheritdoc />
        public async Task MarcarAsistenciaAsync(int idAsistencia, List<ModificarAsistenciaDetalleDTO> cambios)
        {
            if (cambios == null || cambios.Count == 0)
                throw new BadRequestException("Enviá al menos un alumno para modificar.");

            var planilla = await _asistencias.GetByIdAsync(idAsistencia)
                ?? throw new NotFoundException($"No se encontró la planilla {idAsistencia}.");

            if (planilla.Cerrada)
                throw new ConflictException("La planilla está cerrada. Solo administración puede reabrirla.");

            var detalles = await _detalles.ListarPorAlumnosAsync(idAsistencia, cambios.Select(c => c.IdAlumno));

            foreach (var cambio in cambios)
            {
                var detalle = detalles.FirstOrDefault(d => d.IdAlumno == cambio.IdAlumno)
                    ?? throw new BadRequestException($"El alumno {cambio.IdAlumno} no figura en esta planilla.");

                if (cambio.Estado.HasValue) detalle.Estado = cambio.Estado.Value;
                if (cambio.Observacion != null) detalle.Observacion = cambio.Observacion;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ActualizarPlanillaAsync(int idAsistencia, ModificarAsistenciaDTO dto)
        {
            var planilla = await _asistencias.GetByIdAsync(idAsistencia)
                ?? throw new NotFoundException($"No se encontró la planilla {idAsistencia}.");

            if (dto.Cerrada.HasValue)
            {
                if (planilla.Cerrada && !dto.Cerrada.Value && !_currentUser.EsAdmin)
                    throw new ForbiddenException("Solo administración puede reabrir una planilla cerrada.");

                planilla.Cerrada = dto.Cerrada.Value;
            }

            if (dto.Observaciones != null) planilla.Observaciones = dto.Observaciones;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarPlanillaAsync(int idAsistencia)
        {
            var planilla = await _asistencias.GetByIdAsync(idAsistencia)
                ?? throw new NotFoundException($"No se encontró la planilla {idAsistencia}.");

            _asistencias.Remove(planilla);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<ResumenAsistenciaDTO> ResumenAlumnoAsync(int idAlumno, int? ciclo)
        {
            if (!_currentUser.EsPersonal && _currentUser.Id != idAlumno) throw new ForbiddenException();

            return new ResumenAsistenciaDTO
            {
                Anuales = (await _asistencias.ListarInasistenciasAnualesAsync(idAlumno, ciclo)).Select(a => a.ToDTO()).ToList(),
                PorBimestre = (await _asistencias.ListarInasistenciasPorBimestreAsync(idAlumno, ciclo)).Select(p => p.ToDTO()).ToList()
            };
        }
    }
}
