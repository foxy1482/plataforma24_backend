using Plataforma24API.DTOs.Boletines;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de las vistas de boletin a DTO.</summary>
    public static class BoletinMapper
    {
        public static BoletinResumenDTO ToDTO(this VBoletinResumen resumen)
        {
            return new BoletinResumenDTO
            {
                IdBoletin = resumen.IdBoletin,
                Estado = resumen.Estado,
                IdAlumno = resumen.IdAlumno,
                NumeroLegajo = resumen.NumeroLegajo,
                Apellido = resumen.Apellido,
                Nombre = resumen.Nombre,
                IdCurso = resumen.IdCurso,
                CicloLectivo = resumen.CicloLectivo,
                Bimestre = resumen.Bimestre,
                MateriasDelCurso = resumen.MateriasDelCurso,
                MateriasEnBoletin = resumen.MateriasEnBoletin,
                NotasCargadas = resumen.NotasCargadas,
                Promedio = resumen.Promedio,
                MateriasDesaprobadas = resumen.MateriasDesaprobadas,
                InasistenciasBimestre = resumen.InasistenciasBimestre,
                FechaPublicacion = resumen.FechaPublicacion
            };
        }

        public static BoletinMateriaDTO ToDTO(this VBoletinDetalle detalle)
        {
            return new BoletinMateriaDTO
            {
                IdBoletin = detalle.IdBoletin,
                Estado = detalle.Estado,
                CicloLectivo = detalle.CicloLectivo,
                Bimestre = detalle.Bimestre,
                IdAlumno = detalle.IdAlumno,
                NumeroLegajo = detalle.NumeroLegajo,
                Apellido = detalle.Apellido,
                Nombre = detalle.Nombre,
                IdCurso = detalle.IdCurso,
                Curso = detalle.Curso,
                IdMateria = detalle.IdMateria,
                Materia = detalle.Materia,
                IdProfesor = detalle.IdProfesor,
                Profesor = detalle.Profesor,
                Nota = detalle.Nota,
                Observacion = detalle.Observacion,
                FechaCarga = detalle.FechaCarga
            };
        }
    }
}
