namespace Services.IServices;

public interface IPasswordHashService
{
    /// <summary>
    /// Hashea una contraseña usando PBKDF2
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="salt">Salt generado (output)</param>
    /// <returns>Hash de la contraseña</returns>
    string HashPassword(string password, out string salt);

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="storedHash">Hash almacenado</param>
    /// <param name="storedSalt">Salt almacenado</param>
    /// <returns>True si la contraseña es correcta</returns>
    bool VerifyPassword(string password, string storedHash, string storedSalt);
}
