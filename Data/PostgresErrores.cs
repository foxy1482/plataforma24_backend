using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Plataforma24API.Data
{

    public static class PostgresErrores
    {
        public static PostgresException? Buscar(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e is PostgresException pg) return pg;
            }
            return null;
        }

        public static bool EsErrorDeNegocio(Exception ex)
        {
            var pg = Buscar(ex);
            return pg != null &&
                   pg.SqlState is "23514" or "42501" or "FORO1" or "23505" or "23503";
        }

        public static ActionResult RespuestaDeError(this ControllerBase controller, Exception ex)
        {
            var pg = Buscar(ex)!;

            return pg.SqlState switch
            {

                "23514" => controller.BadRequest(new { mensaje = pg.MessageText }),

                "42501" => controller.StatusCode(StatusCodes.Status403Forbidden, new { mensaje = pg.MessageText }),

                "FORO1" => controller.UnprocessableEntity(new { mensaje = pg.MessageText, sugerencia = pg.Hint }),


                "23505" => controller.Conflict(new { mensaje = "Ya existe un registro con esos datos." }),

                "23503" => controller.BadRequest(new { mensaje = "Hace referencia a un registro que no existe, o todavía tiene datos asociados." }),

                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error inesperado en la base de datos." })
            };
        }
    }
}