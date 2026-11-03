using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Materiales;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de materiales didacticos.</summary>
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materiales;
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(IMaterialRepository materiales, IUnitOfWork unitOfWork)
        {
            _materiales = materiales;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MaterialDTO>> ListarAsync(int? idMateria)
        {
            var materiales = await _materiales.ListarAsync(idMateria);
            return materiales.Select(m => m.ToDTO());
        }

        /// <inheritdoc />
        public async Task<MaterialDTO> BuscarAsync(int id)
        {
            var material = await _materiales.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el material {id}.");

            return material.ToDTO();
        }

        /// <inheritdoc />
        public async Task<MaterialDTO> CrearAsync(CrearMaterialDTO dto)
        {
            var material = new Material
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                PublicadaDesde = dto.PublicadaDesde,
                IdMateria = dto.IdMateria
            };

            await _materiales.AddAsync(material);
            await _unitOfWork.SaveChangesAsync();
            return material.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarMaterialDTO dto)
        {
            var material = await _materiales.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el material {id}.");

            if (dto.Titulo != null) material.Titulo = dto.Titulo;
            if (dto.Descripcion != null) material.Descripcion = dto.Descripcion;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var material = await _materiales.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el material {id}.");

            _materiales.Remove(material);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
