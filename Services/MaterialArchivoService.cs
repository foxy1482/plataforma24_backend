using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Materiales;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Archivos asociados a un material didActico.</summary>
    public class MaterialArchivoService : IMaterialArchivoService
    {
        private readonly IMaterialArchivoRepository _vinculos;
        private readonly IMaterialRepository _materiales;
        private readonly IArchivoRepository _archivos;
        private readonly IUnitOfWork _unitOfWork;

        public MaterialArchivoService(
            IMaterialArchivoRepository vinculos,
            IMaterialRepository materiales,
            IArchivoRepository archivos,
            IUnitOfWork unitOfWork)
        {
            _vinculos = vinculos;
            _materiales = materiales;
            _archivos = archivos;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MaterialArchivoDTO>> ListarAsync(int idMaterial)
        {
            var archivos = await _vinculos.ListarArchivosAsync(idMaterial);
            return archivos.Select(a => a.ToMaterialArchivoDTO(idMaterial));
        }

        /// <inheritdoc />
        public async Task VincularAsync(VincMaterialArchivoDTO dto)
        {
            var material = await _materiales.GetByIdAsync(dto.IdMaterial);
            var archivo = await _archivos.GetByIdAsync(dto.IdArchivo);

            if (material == null || archivo == null)
                throw new BadRequestException("El material o el archivo especificado no existen.");

            if (await _vinculos.ExisteVinculoAsync(dto.IdMaterial, dto.IdArchivo))
                throw new BadRequestException("Este archivo ya está vinculado al material.");

            await _vinculos.AddAsync(new MaterialArchivo
            {
                IdMaterial = dto.IdMaterial,
                IdArchivo = dto.IdArchivo
            });
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DesvincularAsync(int idMaterial, int idArchivo)
        {
            var vinculo = await _vinculos.GetVinculoAsync(idMaterial, idArchivo)
                ?? throw new NotFoundException("No se encontró el vínculo entre el material y el archivo.");

            _vinculos.Remove(vinculo);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
