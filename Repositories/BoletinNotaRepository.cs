using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de las notas de un boletin.</summary>
    public class BoletinNotaRepository : Repository<BoletinNota>, IBoletinNotaRepository
    {
        public BoletinNotaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<BoletinNota?> GetAsync(int idBoletin, int idMateria)
        {
            return DbSet.FirstOrDefaultAsync(n => n.IdBoletin == idBoletin && n.IdMateria == idMateria);
        }
    }
}
