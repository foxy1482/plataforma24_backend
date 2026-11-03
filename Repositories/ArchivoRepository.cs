using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de archivos.</summary>
    public class ArchivoRepository : Repository<Archivo>, IArchivoRepository
    {
        public ArchivoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<Archivo>> ListarAsync(string? nombre, int? idUsuario)
        {
            var consulta = DbSet.AsNoTracking();

            if (nombre != null)
            {
                consulta = consulta.Where(a => a.NombreOriginal == nombre);
            }

            if (idUsuario.HasValue)
            {
                consulta = consulta.Where(a => a.IdUsuario == idUsuario.Value);
            }

            return await consulta.ToListAsync();
        }
    }
}
