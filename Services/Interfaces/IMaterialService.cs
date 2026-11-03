using Plataforma24API.DTOs.Materiales;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de materiales didacticos.</summary>
    public interface IMaterialService
    {
        /// <param name="idMateria">Materia; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<IEnumerable<MaterialDTO>> ListarAsync(int? idMateria);

        /// <exception cref="Common.Exceptions.NotFoundException">El material no existe.</exception>
        Task<MaterialDTO> BuscarAsync(int id);

        Task<MaterialDTO> CrearAsync(CrearMaterialDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El material no existe.</exception>
        Task ActualizarAsync(int id, ModificarMaterialDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El material no existe.</exception>
        Task EliminarAsync(int id);
    }
}
