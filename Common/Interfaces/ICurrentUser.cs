namespace Plataforma24API.Common.Interfaces
{
    /// <summary>
    /// Datos del usuario que realiza la peticion actual. Permite que los servicios apliquen reglas de
    /// autorizacion sin depender de <c>HttpContext</c>.
    /// </summary>
    public interface ICurrentUser
    {
        int Id { get; }

        int IdRol { get; }

        bool EsAdmin { get; }

        bool EsProfesor { get; }

        bool EsPreceptor { get; }

        bool EsAlumno { get; }

        /// <summary>Administrativo, profesor o preceptor.</summary>
        bool EsPersonal { get; }

        /// <summary>Indica si el usuario actual es administrador o es el propio usuario objetivo.</summary>
        /// <param name="idUsuarioObjetivo">Usuario sobre el que se quiere actuar.</param>
        bool TienePermiso(int idUsuarioObjetivo);
    }
}
