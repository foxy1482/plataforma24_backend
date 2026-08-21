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
        public AuthProfileDTO Profile(int numLegajo)
        {
            var usuario = _context.Usuarios.Where(u => u.NumeroLegajo == numLegajo).Select(u => new AuthProfileDTO
            {
                NumeroLegajo = u.NumeroLegajo,
                Nombre = u.NombreUsuario,
                Email = u.Email,
                NumRol = u.NumRol,
                NumCurso = u.NumCurso,
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

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

            if (!valid) throw new Exception("Contraseña incorrecta");

            return new AuthResponseDTO { Token=_jwtService.GenerarToken(usuario.NumeroLegajo, usuario.NumRol) };
        }

        public AuthResponseDTO Registro(RegisterDTO dto)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var usuario = new Usuario
            {
                Email = dto.Email,
                NombreUsuario = dto.Nombre,
                PasswordHash = hash,
                NumRol = 1
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.NumeroLegajo, usuario.NumRol) };
        }
    }
}
