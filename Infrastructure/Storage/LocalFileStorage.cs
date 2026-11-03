using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;

namespace Plataforma24API.Infrastructure.Storage
{
    /// <summary>
    /// Almacena los archivos en <c>wwwroot/uploads</c>. Toda ruta recibida se valida contra esa raiz para impedir
    /// que un nombre manipulado acceda a archivos fuera de ella.
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private const int TamanoBuffer = 4096;

        private readonly string _raiz;

        public LocalFileStorage(IWebHostEnvironment entorno)
        {
            var webRoot = entorno.WebRootPath ?? Path.Combine(entorno.ContentRootPath, "wwwroot");
            _raiz = Path.GetFullPath(Path.Combine(webRoot, "uploads"));
        }

        /// <inheritdoc />
        public async Task<string> GuardarAsync(Stream contenido, string? subCarpeta, string nombreArchivo,
            CancellationToken cancellationToken = default)
        {
            var rutaRelativa = Path.Combine(subCarpeta ?? string.Empty, nombreArchivo);
            var rutaCompleta = ResolverRutaSegura(rutaRelativa);

            Directory.CreateDirectory(Path.GetDirectoryName(rutaCompleta)!);

            await using var destino = new FileStream(rutaCompleta, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await contenido.CopyToAsync(destino, cancellationToken);

            return rutaRelativa;
        }

        /// <inheritdoc />
        public bool Existe(string rutaRelativa)
        {
            return File.Exists(ResolverRutaSegura(rutaRelativa));
        }

        /// <inheritdoc />
        public Stream Abrir(string rutaRelativa)
        {
            return new FileStream(ResolverRutaSegura(rutaRelativa), FileMode.Open, FileAccess.Read, FileShare.Read,
                TamanoBuffer, useAsync: true);
        }

        /// <inheritdoc />
        public void Eliminar(string rutaRelativa)
        {
            File.Delete(ResolverRutaSegura(rutaRelativa));
        }

        private string ResolverRutaSegura(string rutaRelativa)
        {
            foreach (var segmento in rutaRelativa.Split('/', '\\'))
            {
                if (segmento.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    throw new BadRequestException("La ruta del archivo contiene caracteres no válidos.");
            }

            var completa = Path.GetFullPath(Path.Combine(_raiz, rutaRelativa));

            var comparacion = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            var raizConSeparador = _raiz.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!completa.StartsWith(raizConSeparador, comparacion))
                throw new BadRequestException("La ruta del archivo no es válida.");

            return completa;
        }
    }
}
