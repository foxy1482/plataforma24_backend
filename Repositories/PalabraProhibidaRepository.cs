using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de palabras bloqueadas. La busqueda en textos se delega en la funcion <c>fn_buscar_palabra_prohibida</c>.</summary>
    public class PalabraProhibidaRepository : Repository<PalabraProhibida>, IPalabraProhibidaRepository
    {
        public PalabraProhibidaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<PalabraProhibida>> ListarOrdenadasAsync()
        {
            return DbSet.AsNoTracking().OrderBy(p => p.Palabra).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<string?> BuscarPalabraProhibidaAsync(string texto)
        {
            var encontrada = await Context.Database
                .SqlQuery<string>($"SELECT COALESCE(public.fn_buscar_palabra_prohibida({texto}), '') AS \"Value\"")
                .SingleAsync();

            return encontrada == "" ? null : encontrada;
        }
    }
}
