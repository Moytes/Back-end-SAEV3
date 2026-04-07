namespace Services.IServices;

public interface IJWTService
{
    Task<string> GenerateToken(Guid userId, string permissions);
}
