using Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Services;

public class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100000; // Número de iteraciones PBKDF2
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Hashea una contraseña usando PBKDF2
    /// </summary>
    public string HashPassword(string password, out string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // Generar salt aleatorio
        byte[] saltBytes = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        // Convertir el salt a Base64 para almacenamiento
        salt = Convert.ToBase64String(saltBytes);

        // Generar hash usando PBKDF2
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            Algorithm,
            HashSize
        );

        // Convertir el hash a Base64 para almacenamiento
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
            return false;

        try
        {
            // Convertir el salt almacenado de Base64 a bytes
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            // Generar hash de la contraseña proporcionada con el salt almacenado
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                Algorithm,
                HashSize
            );

            // Convertir el hash a Base64
            string computedHash = Convert.ToBase64String(hashBytes);

            // Comparar de forma segura (previene timing attacks)
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHash),
                Encoding.UTF8.GetBytes(storedHash)
            );
        }
        catch
        {
            return false;
        }
    }
}