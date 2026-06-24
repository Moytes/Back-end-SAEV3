using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IUserRepositorie userRepositorie,
    IStudentRepositorie studentRepositorie,
    IPasswordHashService passwordHashService,
    IJWTService jwtService) : ControllerBase
{
    private readonly IUserRepositorie _userRepositorie = userRepositorie;
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IJWTService _jwtService = jwtService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var user = await _userRepositorie.GetUserByEmail(request.Email);
        if (user == null)
            return Unauthorized("Invalid or missing user authentication");

        var isPasswordValid = _passwordHashService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
            return Unauthorized("Invalid credentials");

        if (!user.Activo)
            return Unauthorized("User account is inactive");

        var additionalClaims = new List<Claim>();
        if (user.Role.Clave is "TUTOR" or "ALUMNO")
        {
            var portalStudents = await _studentRepositorie.GetPortalStudentsByUser(user.Id, user.Role.Clave);
            additionalClaims.AddRange(portalStudents.Select(student => new Claim("student_id", student.Id.ToString())));
        }

        var token = await _jwtService.GenerateToken(user.Id, user.Role.Clave, additionalClaims);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(
                double.Parse(HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Jwt:ExpirationInMinutes"]!))
        });

        return Ok(new
        {
            role = user.Role.Clave,
            token
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok("Logout successful");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var user = await _userRepositorie.GetUserById(userId);
        if (user == null)
            return Unauthorized("User not found");

        var schoolIds = await _userRepositorie.GetUserSchools(userId);

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Name,
            user.FatherLastName,
            user.MotherLastName,
            RoleId = user.RoleId,
            RoleClave = user.Role.Clave,
            RoleNombre = user.Role.Nombre,
            user.Phone,
            user.Activo,
            user.AvatarUrl,
            user.CreatedAt,
            user.UpdatedAt,
            schoolIds,
            schoolZoneId = user.SchoolZoneId
        });
    }

    [HttpGet("HealthCheck")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok("API is healthy");
    }
}
