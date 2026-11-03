using Plataforma24API.Common.Interfaces;

namespace Plataforma24API.Infrastructure.Security
{
    /// <summary>Cifra contrasenas con BCrypt, que incorpora sal y factor de costo configurables.</summary>
    public class BCryptPasswordHasher : IPasswordHasher
    {
        /// <inheritdoc />
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <inheritdoc />
        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
