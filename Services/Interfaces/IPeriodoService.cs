using Plataforma24API.DTOs.Periodos;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de los periodos bimestrales de cada ciclo lectivo.</summary>
    public interface IPeriodoService
    {
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        Task<IEnumerable<PeriodoDTO>> ListarAsync(int? ciclo);

        /// <exception cref="Common.Exceptions.NotFoundException">El periodo no existe.</exception>
        Task<PeriodoDTO> BuscarAsync(int id);

        /// <exception cref="Common.Exceptions.BadRequestException">Bimestre fuera de 1 a 4, fechas incoherentes o periodo repetido.</exception>
        Task<PeriodoDTO> CrearAsync(PeriodoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El periodo no existe.</exception>
        /// <exception cref="Common.Exceptions.BadRequestException">La fecha de fin no es posterior a la de inicio.</exception>
        Task ActualizarAsync(int id, ModificarPeriodoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El periodo no existe.</exception>
        Task EliminarAsync(int id);
    }
}
