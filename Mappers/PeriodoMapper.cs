using Plataforma24API.DTOs.Periodos;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="PeriodoBimestral"/> a DTO.</summary>
    public static class PeriodoMapper
    {
        public static PeriodoDTO ToDTO(this PeriodoBimestral periodo)
        {
            return new PeriodoDTO
            {
                Id = periodo.Id,
                CicloLectivo = periodo.CicloLectivo,
                Bimestre = periodo.Bimestre,
                FechaInicio = periodo.FechaInicio,
                FechaFin = periodo.FechaFin
            };
        }
    }
}
