using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de mensajes del foro.</summary>
    public class ForoMensajeRepository : Repository<ForoMensaje>, IForoMensajeRepository
    {
        public ForoMensajeRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<VForoMensaje>> ListarVistaPorTemaAsync(int idTema)
        {
            return Context.VForoMensajes
                .Where(m => m.IdTema == idTema)
                .OrderBy(m => m.FechaPublicacion)
                .ToListAsync();
        }
    }
}
