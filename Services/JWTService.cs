using Microsoft.IdentityModel.Tokens;
using Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class JWTService(IConfiguration configuration) : IJWTService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GenerateToken(Guid userId, string roleClave, IEnumerable<Claim>? additionalClaims = null)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "SuperSecretKeyForSIAEBackendAuthentication2026!#";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, roleClave)
        };

        if (additionalClaims != null)
            claims.AddRange(additionalClaims);

        var issuer = _configuration["Jwt:Issuer"] ?? "SIAE-Backend";
        var audience = _configuration["Jwt:Audience"] ?? "SIAE-Frontend";
        var expMinutesStr = _configuration["Jwt:ExpirationInMinutes"];
        var expMinutes = double.TryParse(expMinutesStr, out var m) ? m : 1440;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: credentials);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
