namespace Plataforma24API.DTOs.Archivos
{
    /// <summary>
    /// Contenido de un archivo listo para ser enviado al cliente.
    /// Quien lo recibe debe liberar <see cref="Contenido"/>; el framework lo hace al responder con <c>File(...)</c>.
    /// </summary>
    public class ArchivoContenido
    {
        public Stream Contenido { get; set; } = null!;

        /// <summary>Tipo MIME del archivo.</summary>
        public string Tipo { get; set; } = null!;

        public string NombreOriginal { get; set; } = null!;
    }
}
