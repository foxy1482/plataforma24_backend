using Plataforma24API.DTOs.Foro;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de las entidades y vistas del foro a DTO.</summary>
    public static class ForoMapper
    {
        public static PalabraDTO ToDTO(this PalabraProhibida palabra)
        {
            return new PalabraDTO
            {
                Id = palabra.Id,
                Palabra = palabra.Palabra,
                Modo = palabra.Modo,
                Activa = palabra.Activa,
                IdAgregadaPor = palabra.IdAgregadaPor,
                FechaAlta = palabra.FechaAlta
            };
        }

        public static ForoTemaDTO ToDTO(this VForoTema tema)
        {
            return new ForoTemaDTO
            {
                Id = tema.Id,
                IdCurso = tema.IdCurso,
                Titulo = tema.Titulo,
                IdAutor = tema.IdAutor,
                Autor = tema.Autor,
                FechaCreacion = tema.FechaCreacion,
                Fijado = tema.Fijado,
                Cerrado = tema.Cerrado,
                CantidadMensajes = tema.CantidadMensajes,
                UltimoMensaje = tema.UltimoMensaje
            };
        }

        public static ForoMensajeDTO ToDTO(this VForoMensaje mensaje)
        {
            return new ForoMensajeDTO
            {
                Id = mensaje.Id,
                IdTema = mensaje.IdTema,
                Tema = mensaje.Tema,
                IdCurso = mensaje.IdCurso,
                IdAutor = mensaje.IdAutor,
                Autor = mensaje.Autor,
                RolAutor = mensaje.RolAutor,
                IdRespuestaA = mensaje.IdRespuestaA,
                Contenido = mensaje.Contenido,
                FechaPublicacion = mensaje.FechaPublicacion,
                FechaEdicion = mensaje.FechaEdicion,
                Eliminado = mensaje.Eliminado,
                ReportesPendientes = mensaje.ReportesPendientes
            };
        }

        public static ReporteDTO ToDTO(this ForoReporte reporte, ForoMensaje mensaje)
        {
            return new ReporteDTO
            {
                Id = reporte.Id,
                IdMensaje = reporte.IdMensaje,
                ContenidoMensaje = mensaje.Contenido,
                IdAutorMensaje = mensaje.IdAutor,
                IdReportante = reporte.IdReportante,
                Motivo = reporte.Motivo,
                Fecha = reporte.Fecha,
                Resuelto = reporte.Resuelto,
                IdResueltoPor = reporte.IdResueltoPor
            };
        }
    }
}
