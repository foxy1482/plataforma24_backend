using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Auth;
using Plataforma24API.Models;

namespace Plataforma24API.Auth
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        public AuthProfileDTO Profile(int idUsuario)
        {
            var usuario = _context.Usuarios.Where(u => u.Id == idUsuario).Select(u => new AuthProfileDTO
            {
                Id = u.Id,
                NumeroLegajo = u.NumeroLegajo,
                Nombre = u.NombreUsuario,
                Email = u.Email,
                IdRol = u.IdRol,
                IdCurso = u.IdCurso,
            }).FirstOrDefault();

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }
            return usuario ?? throw new Exception("Usuario no encontrado en la base de datos");
        }
        public AuthResponseDTO Login(LoginDTO dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == dto.Email);

            if (usuario == null) throw new Exception("Usuario no encontrado");

            if (!usuario.Activo) throw new Exception("El usuario está inactivo");

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

            if (!valid) throw new Exception("Contraseña incorrecta");

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.Id, usuario.IdRol) };
        }

        public AuthResponseDTO Registro(RegisterDTO dto)
        {

            if (_context.Usuarios.Any(u => u.Email == dto.Email))
                throw new Exception("El email ya está registrado");

            if (_context.Usuarios.Any(u => u.NombreUsuario == dto.Nombre))
                throw new Exception("El nombre de usuario ya está en uso");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var usuario = new Usuario
            {
                Email = dto.Email,
                NombreUsuario = dto.Nombre,
                PasswordHash = hash,
                IdRol = 1
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.Id, usuario.IdRol) };
        }
    }
}