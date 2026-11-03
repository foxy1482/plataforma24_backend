namespace Plataforma24API.Common.Constants
{
    /// <summary>
    /// Identificadores de los roles cargados en la tabla <c>rol</c> y reglas de pertenencia a cada grupo.
    /// </summary>
    public static class RolIds
    {
        public const int Alumno = 1;
        public const int Profesor = 2;
        public const int Autoridad = 3;
        public const int AdminAlumno = 4;
        public const int AdminProfesor = 5;
        public const int Admin = 6;
        public const int Preceptor = 7;

        /// <summary>Indica si el rol es administrativo (Autoridad, AdminAlumno, AdminProfesor o Admin).</summary>
        public static bool EsAdmin(int idRol) => idRol is >= Autoridad and <= Admin;

        public static bool EsProfesor(int idRol) => idRol == Profesor;

        public static bool EsPreceptor(int idRol) => idRol == Preceptor;

        /// <summary>Indica si el rol es de alumno, incluido el alumno con permisos administrativos.</summary>
        public static bool EsAlumno(int idRol) => idRol == Alumno || idRol == AdminAlumno;

        /// <summary>Indica si el rol pertenece al personal de la institucion (administrativo, profesor o preceptor).</summary>
        public static bool EsPersonal(int idRol) => EsAdmin(idRol) || EsProfesor(idRol) || EsPreceptor(idRol);

        public static bool EsAdminOPreceptor(int idRol) => EsAdmin(idRol) || EsPreceptor(idRol);

        public static bool EsAdminOProfesor(int idRol) => EsAdmin(idRol) || EsProfesor(idRol);
    }
}
