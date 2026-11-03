using Plataforma24API.DTOs.Archivos;
using Plataforma24API.DTOs.Entregas;
using Plataforma24API.DTOs.Materiales;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Archivo"/> a los DTO que lo exponen.</summary>
    public static class ArchivoMapper
    {
        public static ArchivoDTO ToDTO(this Archivo archivo)
        {
            return new ArchivoDTO
            {
                Id = archivo.Id,
                NombreOriginal = archivo.NombreOriginal,
                GuidArchivo = archivo.GuidArchivo,
                Ruta = archivo.Ruta,
                Tipo = archivo.Tipo,
                IdUsuario = archivo.IdUsuario,
                FechaSubida = archivo.FechaSubida
            };
        }

        public static MaterialArchivoDTO ToMaterialArchivoDTO(this Archivo archivo, int idMaterial)
        {
            return new MaterialArchivoDTO
            {
                IdMaterial = idMaterial,
                IdArchivo = archivo.Id,
                NombreOriginal = archivo.NombreOriginal,
                Ruta = archivo.Ruta
            };
        }

        public static TareaArchivoDTO ToTareaArchivoDTO(this Archivo archivo, int idTarea)
        {
            return new TareaArchivoDTO
            {
                IdTarea = idTarea,
                IdArchivo = archivo.Id,
                NombreOriginal = archivo.NombreOriginal,
                Ruta = archivo.Ruta
            };
        }

        public static EntregaArchivoDTO ToEntregaArchivoDTO(this Archivo archivo, int idEntrega)
        {
            return new EntregaArchivoDTO
            {
                IdEntrega = idEntrega,
                IdArchivo = archivo.Id,
                NombreOriginal = archivo.NombreOriginal,
                Ruta = archivo.Ruta
            };
        }
    }
}
