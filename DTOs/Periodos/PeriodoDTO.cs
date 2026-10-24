namespace Plataforma24API.DTOs.Periodos
{
    public class PeriodoDTO
    {
        public int Id { get; set; } 

        public int CicloLectivo { get; set; }

        public short Bimestre { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }
    }

    public class ModificarPeriodoDTO
    {
        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }
    }
}