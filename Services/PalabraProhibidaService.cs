using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Foro;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de palabras bloqueadas y verificacion de textos.</summary>
    public class PalabraProhibidaService : IPalabraProhibidaService
    {
        private readonly IPalabraProhibidaRepository _palabras;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public PalabraProhibidaService(
            IPalabraProhibidaRepository palabras,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _palabras = palabras;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PalabraDTO>> ListarAsync()
        {
            var palabras = await _palabras.ListarOrdenadasAsync();
            return palabras.Select(p => p.ToDTO());
        }

        /// <inheritdoc />
        public async Task<PalabraDTO> CrearAsync(CrearPalabraDTO dto)
        {
            var palabra = new PalabraProhibida
            {
                Palabra = dto.Palabra.Trim(),
                Modo = dto.Modo,
                IdAgregadaPor = _currentUser.Id
            };

            await _palabras.AddAsync(palabra);
            await _unitOfWork.SaveChangesAsync();

            return palabra.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarPalabraDTO dto)
        {
            var palabra = await _palabras.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la palabra {id}.");

            if (dto.Activa.HasValue) palabra.Activa = dto.Activa.Value;
            if (dto.Modo.HasValue) palabra.Modo = dto.Modo.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var palabra = await _palabras.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la palabra {id}.");

            _palabras.Remove(palabra);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<VerificacionTextoDTO> VerificarAsync(VerificarTextoDTO dto)
        {
            var encontrada = await _palabras.BuscarPalabraProhibidaAsync(dto.Texto);

            return new VerificacionTextoDTO
            {
                Permitido = encontrada == null,
                Palabra = encontrada
            };
        }
    }
}
