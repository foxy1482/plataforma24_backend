using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion entre <see cref="DatosPersonales"/> y su DTO.</summary>
    public static class DatosPersonalesMapper
    {
        public static DatosPersonalesDTO ToDTO(this DatosPersonales datos)
        {
            return new DatosPersonalesDTO
            {
                IdUsuario = datos.IdUsuario,
                Nombre = datos.Nombre,
                Apellido = datos.Apellido,
                DNI = datos.DNI,
                TelefonoCelular = datos.TelefonoCelular,
                Direccion = datos.Direccion,
                FechaNacimiento = datos.FechaNacimiento,
                IdFotoArchivo = datos.IdFotoArchivo,
                Sexo = datos.Sexo
            };
        }

        public static DatosPersonales ToEntity(this DatosPersonalesDTO dto)
        {
            return new DatosPersonales
            {
                IdUsuario = dto.IdUsuario,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                TelefonoCelular = dto.TelefonoCelular,
                Direccion = dto.Direccion,
                FechaNacimiento = dto.FechaNacimiento,
                IdFotoArchivo = dto.IdFotoArchivo,
                Sexo = dto.Sexo
            };
        }
    }
}
