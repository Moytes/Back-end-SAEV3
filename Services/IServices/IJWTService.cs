namespace Services.IServices;

public interface IJWTService
{
    Task<string> GenerateToken(Guid userId, string roleClave, IEnumerable<System.Security.Claims.Claim>? additionalClaims = null);
}
