using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Entregas;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Archivos adjuntos de las entregas. Solo el autor de la entrega (o un administrador) puede modificarlos.</summary>
    public class EntregaArchivoService : IEntregaArchivoService
    {
        private readonly IEntregaArchivoRepository _vinculos;
        private readonly IEntregaRepository _entregas;
        private readonly IArchivoRepository _archivos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public EntregaArchivoService(
            IEntregaArchivoRepository vinculos,
            IEntregaRepository entregas,
            IArchivoRepository archivos,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _vinculos = vinculos;
            _entregas = entregas;
            _archivos = archivos;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EntregaArchivoDTO>> ListarAsync(int idEntrega)
        {
            var entrega = await _entregas.GetByIdAsync(idEntrega)
                ?? throw new NotFoundException("La entrega especificada no existe.");

            if (!_currentUser.EsProfesor && !_currentUser.TienePermiso(entrega.IdUsuario))
                throw new ForbiddenException("No tenés permiso para ver los archivos de esta entrega.");

            var archivos = await _vinculos.ListarArchivosAsync(idEntrega);
            return archivos.Select(a => a.ToEntregaArchivoDTO(idEntrega));
        }

        /// <inheritdoc />
        public async Task VincularAsync(VincEntregaArchivoDTO dto)
        {
            var entrega = await _entregas.GetByIdAsync(dto.IdEntrega);
            var archivo = await _archivos.GetByIdAsync(dto.IdArchivo);

            if (entrega == null || archivo == null)
                throw new BadRequestException("La entrega o el archivo especificado no existen.");

            if (!_currentUser.TienePermiso(entrega.IdUsuario))
                throw new ForbiddenException("Solo podés agregarle archivos a tu propia entrega.");

            if (!_currentUser.EsAdmin && archivo.IdUsuario != _currentUser.Id)
                throw new BadRequestException("No podés vincular un archivo que no te pertenece.");

            if (await _vinculos.ExisteVinculoAsync(dto.IdEntrega, dto.IdArchivo))
                throw new BadRequestException("Este archivo ya está vinculado a la entrega.");

            await _vinculos.AddAsync(new EntregaArchivo
            {
                IdEntrega = dto.IdEntrega,
                IdArchivo = dto.IdArchivo
            });
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DesvincularAsync(int idEntrega, int idArchivo)
        {
            var vinculo = await _vinculos.GetVinculoAsync(idEntrega, idArchivo);

            var entrega = await _entregas.GetByIdAsync(idEntrega);
            if (!_currentUser.EsAdmin && (entrega == null || !_currentUser.TienePermiso(entrega.IdUsuario)))
                throw new ForbiddenException("No tenés permiso para modificar esta entrega.");

            var archivo = await _archivos.GetByIdAsync(idArchivo);
            if (!_currentUser.EsAdmin && archivo != null && archivo.IdUsuario != _currentUser.Id)
                throw new ForbiddenException("No podés quitar un archivo que no subiste vos.");

            if (vinculo == null)
                throw new NotFoundException("No se encontró el vínculo entre la entrega y el archivo.");

            _vinculos.Remove(vinculo);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DesvincularTodosAsync(int idEntrega)
        {
            var entrega = await _entregas.GetByIdAsync(idEntrega)
                ?? throw new NotFoundException("La entrega no existe.");

            if (!_currentUser.TienePermiso(entrega.IdUsuario))
                throw new ForbiddenException("No podés modificar los archivos de esta entrega.");

            var vinculos = await _vinculos.ListarVinculosAsync(idEntrega);

            if (vinculos.Count > 0)
            {
                _vinculos.RemoverVarios(vinculos);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
