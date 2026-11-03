using Plataforma24API.DTOs.Materiales;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Archivos asociados a los materiales didacticos.</summary>
    public interface IMaterialArchivoService
    {
        Task<IEnumerable<MaterialArchivoDTO>> ListarAsync(int idMaterial);

        /// <exception cref="Common.Exceptions.BadRequestException">El material o el archivo no existen, o ya estaban asociados.</exception>
        Task VincularAsync(VincMaterialArchivoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no esta asociado al material.</exception>
        Task DesvincularAsync(int idMaterial, int idArchivo);
    }
}
