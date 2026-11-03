namespace Plataforma24API.Common.Interfaces
{
    /// <summary>
    /// Abstraccion del almacenamiento fisico de archivos. Permite reemplazar el disco local
    /// por otro medio (por ejemplo, un bucket en la nube) sin modificar los servicios.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>Guarda el contenido y devuelve la ruta relativa con la que se lo puede recuperar.</summary>
        /// <param name="contenido">Flujo con los bytes del archivo.</param>
        /// <param name="subCarpeta">Carpeta opcional dentro del almacenamiento.</param>
        /// <param name="nombreArchivo">Nombre con el que se guarda, ya saneado y unico.</param>
        /// <param name="cancellationToken">Token de cancelacion.</param>
        /// <returns>Ruta relativa al almacenamiento.</returns>
        Task<string> GuardarAsync(Stream contenido, string? subCarpeta, string nombreArchivo,
            CancellationToken cancellationToken = default);

        bool Existe(string rutaRelativa);

        /// <summary>Abre el archivo para lectura. Quien lo recibe es responsable de liberarlo.</summary>
        Stream Abrir(string rutaRelativa);

        void Eliminar(string rutaRelativa);
    }
}
