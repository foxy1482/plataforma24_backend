namespace Plataforma24API.Common.Interfaces
{
    /// <summary>Cifrado y verificacion de contrasenas.</summary>
    public interface IPasswordHasher
    {
        /// <summary>Devuelve el hash de la contrasena en texto plano.</summary>
        string Hash(string password);

        /// <summary>Comprueba si la contrasena en texto plano corresponde al hash guardado.</summary>
        bool Verify(string password, string hash);
    }
}
