namespace Plataforma24API.DTOs.Periodos
{
    /// <summary>Bimestre de un ciclo lectivo. Al crear un periodo, el campo <c>Id</c> se ignora.</summary>
    public class PeriodoDTO
    {
        public int Id { get; set; }

        public int CicloLectivo { get; set; }

        public short Bimestre { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }
    }

    /// <summary>Nuevas fechas de un periodo. Los campos nulos no se modifican.</summary>
    public class ModificarPeriodoDTO
    {
        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }
    }
}
