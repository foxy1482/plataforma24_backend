using Plataforma24API.DTOs.Auth;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Usuario"/> a DTO.</summary>
    public static class UsuarioMapper
    {
        public static UsuarioDTO ToDTO(this Usuario usuario)
        {
            return new UsuarioDTO
            {
                Id = usuario.Id,
                NumeroLegajo = usuario.NumeroLegajo,
                Email = usuario.Email,
                NombreUsuario = usuario.NombreUsuario,
                IdRol = usuario.IdRol,
                IdCurso = usuario.IdCurso
            };
        }

        public static AuthProfileDTO ToProfileDTO(this Usuario usuario)
        {
            return new AuthProfileDTO
            {
                Id = usuario.Id,
                NumeroLegajo = usuario.NumeroLegajo,
                Nombre = usuario.NombreUsuario,
                Email = usuario.Email,
                IdRol = usuario.IdRol,
                IdCurso = usuario.IdCurso
            };
        }
    }
}
