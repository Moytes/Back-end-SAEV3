using Microsoft.IdentityModel.Tokens;
using Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class JWTService(IConfiguration configuration) : IJWTService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GenerateToken(Guid userId, string role, Guid? studentId = null)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        if (studentId.HasValue)
        {
            claims.Add(new Claim("studentId", studentId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpirationInMinutes"]!)),
            signingCredentials: credentials);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
