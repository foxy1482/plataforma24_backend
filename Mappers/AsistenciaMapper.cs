using Plataforma24API.DTOs.Asistencias;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de asistencias y sus vistas a DTO.</summary>
    public static class AsistenciaMapper
    {
        public static AsistenciaDTO ToDTO(this Asistencia asistencia)
        {
            return new AsistenciaDTO
            {
                Id = asistencia.Id,
                IdCurso = asistencia.IdCurso,
                Fecha = asistencia.Fecha,
                Cerrada = asistencia.Cerrada,
                IdRegistradoPor = asistencia.IdRegistradoPor,
                Observaciones = asistencia.Observaciones
            };
        }

        public static AsistenciaDetalleDTO ToDTO(this AsistenciaDetalle detalle, Usuario alumno, DatosPersonales? datos)
        {
            return new AsistenciaDetalleDTO
            {
                Id = detalle.Id,
                IdAlumno = detalle.IdAlumno,
                NumeroLegajo = alumno.NumeroLegajo,
                Apellido = datos?.Apellido,
                Nombre = datos?.Nombre,
                Estado = detalle.Estado,
                Observacion = detalle.Observacion
            };
        }

        public static InasistenciaAnualDTO ToDTO(this VInasistenciaAnual vista)
        {
            return new InasistenciaAnualDTO
            {
                IdAlumno = vista.IdAlumno,
                CicloLectivo = vista.CicloLectivo,
                Ausentes = vista.Ausentes,
                Tardes = vista.Tardes,
                Justificadas = vista.Justificadas,
                TotalInasistencias = vista.TotalInasistencias,
                DiasRegistrados = vista.DiasRegistrados
            };
        }

        public static InasistenciaPeriodoDTO ToDTO(this VInasistenciaPeriodo vista)
        {
            return new InasistenciaPeriodoDTO
            {
                IdAlumno = vista.IdAlumno,
                IdPeriodo = vista.IdPeriodo,
                CicloLectivo = vista.CicloLectivo,
                Bimestre = vista.Bimestre,
                Presentes = vista.Presentes,
                Ausentes = vista.Ausentes,
                Tardes = vista.Tardes,
                Justificadas = vista.Justificadas,
                TotalInasistencias = vista.TotalInasistencias,
                DiasRegistrados = vista.DiasRegistrados
            };
        }

        public static PlanillaAsistenciaDTO ToPlanillaDTO(this Asistencia asistencia, List<AsistenciaDetalleDTO> detalle)
        {
            return new PlanillaAsistenciaDTO
            {
                Id = asistencia.Id,
                IdCurso = asistencia.IdCurso,
                Fecha = asistencia.Fecha,
                Cerrada = asistencia.Cerrada,
                IdRegistradoPor = asistencia.IdRegistradoPor,
                Observaciones = asistencia.Observaciones,
                Detalle = detalle
            };
        }
    }
}
