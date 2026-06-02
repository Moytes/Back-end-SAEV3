using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;

namespace Controllers;

/// <summary>
/// Authentication controller for user login and logout operations
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(
    IUserRepositorie userRepositorie,
    IPasswordHashService passwordHashService,
    IJWTService jwtService) : ControllerBase
{
    private readonly IUserRepositorie _userRepositorie = userRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IJWTService _jwtService = jwtService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        // Get user by email
        var user = await _userRepositorie.GetUserByEmail(request.Email);
        if (user == null)
            return Unauthorized("Invalid or missing user authentication");

        // Verify password
        var isPasswordValid = _passwordHashService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
            return Unauthorized("Invalid credentials");

        // Check if user is active
        if (user.Status != Models.DB.BoolStatus.True)
            return Unauthorized("User account is inactive");

        // Generate JWT token
        var token = await _jwtService.GenerateToken(user.Id, user.Role.ToString(), user.StudentId);

        // Set JWT in HTTP-only cookie
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Only sent over HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(
                double.Parse(HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Jwt:ExpirationInMinutes"]!))
        });

        return Ok(new
        {
            role = user.Role.ToString(),
            token = token
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Remove JWT cookie
        Response.Cookies.Delete("jwt");

        return Ok("Logout successful");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        // Get user ID from claims
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        // Get user from database
        var user = await _userRepositorie.GetUserById(userId);

        if (user == null)
            return Unauthorized("User not found");

        // Get assigned schools for this user
        var schoolIds = await _userRepositorie.GetUserSchools(userId);

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Name,
            user.FatherLastName,
            user.MotherLastName,
            user.Role,
            user.PhoneNumber,
            user.Status,
            user.AvatarUrl,
            user.CreatedAt,
            user.UpdatedAt,
            schoolIds = schoolIds,
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

