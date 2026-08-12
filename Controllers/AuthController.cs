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
    IJWTService jwtService,
    IConfiguration configuration) : ControllerBase
{
    private readonly IUserRepositorie _userRepositorie = userRepositorie;
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IJWTService _jwtService = jwtService;
    private readonly IConfiguration _configuration = configuration;

    private const string AuthCookieName = "jwt";

    /// <summary>
    /// Opciones de la cookie de autenticación. httpOnly siempre; Secure y SameSite
    /// son configurables (Auth:Cookie:*) con valores seguros por defecto, para poder
    /// usar SameSite=None en despliegues cross-domain sin tocar código.
    /// </summary>
    private CookieOptions BuildAuthCookieOptions(bool withExpiry)
    {
        var sameSite = Enum.TryParse<SameSiteMode>(
            _configuration["Auth:Cookie:SameSite"], ignoreCase: true, out var parsed)
            ? parsed
            : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = _configuration.GetValue("Auth:Cookie:Secure", true),
            SameSite = sameSite,
            Path = "/"
        };

        if (withExpiry)
            options.Expires = DateTimeOffset.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpirationInMinutes"]!));

        return options;
    }

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

        Response.Cookies.Append(AuthCookieName, token, BuildAuthCookieOptions(withExpiry: true));

        return Ok(new
        {
            role = user.Role.Clave,
            token
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Borrar con los mismos atributos (Path/Secure/SameSite) para garantizar
        // que el navegador elimine la cookie.
        Response.Cookies.Delete(AuthCookieName, BuildAuthCookieOptions(withExpiry: false));
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
            user.Especialidad,
            user.CreatedAt,
            user.UpdatedAt,
            schoolIds,
            schoolZoneId = user.SchoolZoneId
        });
    }

    /// <summary>
    /// Autoedición de perfil — solo los campos que le pertenecen al propio usuario
    /// (nombre, apellidos, teléfono, avatar). Rol/zona/estado activo no se pueden
    /// tocar desde aquí; eso sigue siendo exclusivo de PUT /api/usuarios/{id} (ADMIN).
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMyProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var user = await _userRepositorie.GetUserById(userId);
        if (user == null)
            return Unauthorized("User not found");

        var updateRequest = new UpdateUserRequest
        {
            Email = user.Email,
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            RoleId = user.RoleId,
            SchoolZoneId = user.SchoolZoneId,
            AcademySubscriptionId = user.AcademySubscriptionId,
            Phone = request.Phone,
            AvatarUrl = request.AvatarUrl,
            Activo = user.Activo
        };

        var result = await _userRepositorie.UpdateUser(userId, updateRequest);
        if (!result.IsSuccess)
            return BadRequest(result.error.Message);

        return await GetMe();
    }

    /// <summary>
    /// Cambio de contraseña propia — requiere conocer la contraseña actual, a diferencia
    /// del alta de usuario (ADMIN) que asigna una directamente.
    /// </summary>
    [HttpPost("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var user = await _userRepositorie.GetUserById(userId);
        if (user == null)
            return Unauthorized("User not found");

        if (!_passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            return BadRequest("La contraseña actual no es correcta.");

        var newHash = _passwordHashService.HashPassword(request.NewPassword, out var newSalt);
        var result = await _userRepositorie.UpdatePassword(userId, newHash, newSalt);
        if (!result.IsSuccess)
            return BadRequest(result.error.Message);

        return Ok("Contraseña actualizada correctamente.");
    }

    [HttpGet("HealthCheck")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok("API is healthy");
    }
}
