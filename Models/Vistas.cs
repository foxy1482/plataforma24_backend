using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{

    public class VBoletinResumen
    {
        [Column("id_boletin")] public int IdBoletin { get; set; }
        [Column("estado")] public EstadoBoletin Estado { get; set; }
        [Column("id_alumno")] public int IdAlumno { get; set; }
        [Column("numero_legajo")] public string NumeroLegajo { get; set; } = null!;
        [Column("apellido")] public string? Apellido { get; set; }
        [Column("nombre")] public string? Nombre { get; set; }
        [Column("id_curso")] public int IdCurso { get; set; }
        [Column("ciclo_lectivo")] public int CicloLectivo { get; set; }
        [Column("bimestre")] public short Bimestre { get; set; }
        [Column("materias_del_curso")] public long MateriasDelCurso { get; set; }
        [Column("materias_en_boletin")] public long MateriasEnBoletin { get; set; }
        [Column("notas_cargadas")] public long NotasCargadas { get; set; }
        [Column("promedio")] public decimal? Promedio { get; set; }
        [Column("materias_desaprobadas")] public long MateriasDesaprobadas { get; set; }
        [Column("inasistencias_bimestre")] public decimal InasistenciasBimestre { get; set; }
        [Column("fecha_publicacion")] public DateTime? FechaPublicacion { get; set; }
    }

    public class VBoletinDetalle
    {
        [Column("id_boletin")] public int IdBoletin { get; set; }
        [Column("estado")] public EstadoBoletin Estado { get; set; }
        [Column("ciclo_lectivo")] public int CicloLectivo { get; set; }
        [Column("bimestre")] public short Bimestre { get; set; }
        [Column("id_alumno")] public int IdAlumno { get; set; }
        [Column("numero_legajo")] public string NumeroLegajo { get; set; } = null!;
        [Column("apellido")] public string? Apellido { get; set; }
        [Column("nombre")] public string? Nombre { get; set; }
        [Column("id_curso")] public int IdCurso { get; set; }
        [Column("curso")] public string Curso { get; set; } = null!;
        [Column("id_materia")] public int IdMateria { get; set; }
        [Column("materia")] public string Materia { get; set; } = null!;
        [Column("id_profesor")] public int? IdProfesor { get; set; }
        [Column("profesor")] public string? Profesor { get; set; }
        [Column("nota")] public decimal? Nota { get; set; }
        [Column("observacion")] public string? Observacion { get; set; }
        [Column("fecha_carga")] public DateTime? FechaCarga { get; set; }
    }

    public class VInasistenciaPeriodo
    {
        [Column("id_alumno")] public int IdAlumno { get; set; }
        [Column("id_periodo")] public int IdPeriodo { get; set; }
        [Column("ciclo_lectivo")] public int CicloLectivo { get; set; }
        [Column("bimestre")] public short Bimestre { get; set; }
        [Column("presentes")] public long Presentes { get; set; }
        [Column("ausentes")] public long Ausentes { get; set; }
        [Column("tardes")] public long Tardes { get; set; }
        [Column("justificadas")] public long Justificadas { get; set; }
        [Column("total_inasistencias")] public decimal TotalInasistencias { get; set; }
        [Column("dias_registrados")] public long DiasRegistrados { get; set; }
    }

    public class VInasistenciaAnual
    {
        [Column("id_alumno")] public int IdAlumno { get; set; }
        [Column("ciclo_lectivo")] public int CicloLectivo { get; set; }
        [Column("ausentes")] public long Ausentes { get; set; }
        [Column("tardes")] public long Tardes { get; set; }
        [Column("justificadas")] public long Justificadas { get; set; }
        [Column("total_inasistencias")] public decimal TotalInasistencias { get; set; }
        [Column("dias_registrados")] public long DiasRegistrados { get; set; }
    }


    public class VForoTema
    {
        [Column("id")] public int Id { get; set; }
        [Column("id_curso")] public int? IdCurso { get; set; }
        [Column("titulo")] public string Titulo { get; set; } = null!;
        [Column("id_autor")] public int IdAutor { get; set; }
        [Column("autor")] public string Autor { get; set; } = null!;
        [Column("fecha_creacion")] public DateTime FechaCreacion { get; set; }
        [Column("fijado")] public bool Fijado { get; set; }
        [Column("cerrado")] public bool Cerrado { get; set; }
        [Column("cantidad_mensajes")] public long CantidadMensajes { get; set; }
        [Column("ultimo_mensaje")] public DateTime? UltimoMensaje { get; set; }
    }

    public class VForoMensaje
    {
        [Column("id")] public int Id { get; set; }
        [Column("id_tema")] public int IdTema { get; set; }
        [Column("tema")] public string Tema { get; set; } = null!;
        [Column("id_curso")] public int? IdCurso { get; set; }
        [Column("id_autor")] public int IdAutor { get; set; }
        [Column("autor")] public string Autor { get; set; } = null!;
        [Column("rol_autor")] public string RolAutor { get; set; } = null!;
        [Column("id_respuesta_a")] public int? IdRespuestaA { get; set; }
        [Column("contenido")] public string Contenido { get; set; } = null!;
        [Column("fecha_publicacion")] public DateTime FechaPublicacion { get; set; }
        [Column("fecha_edicion")] public DateTime? FechaEdicion { get; set; }
        [Column("eliminado")] public bool Eliminado { get; set; }
        [Column("reportes_pendientes")] public long ReportesPendientes { get; set; }
    }
}