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

    // Helpers for standardized responses
    private IActionResult StandardSuccess(int httpStatusCode, string message, object? data = null)
    {
        var responseData = data switch
        {
            null => Array.Empty<object>(),
            System.Collections.IEnumerable enumerable when data is not string => enumerable.Cast<object>().ToArray(),
            _ => new[] { data }
        };

        return StatusCode(httpStatusCode, new
        {
            statusCode = httpStatusCode,
            message,
            data = responseData
        });
    }

    private IActionResult StandardError(int httpStatusCode, string message)
    {
        return StatusCode(httpStatusCode, new
        {
            statusCode = httpStatusCode,
            message,
            data = Array.Empty<object>()
        });
    }

    private IActionResult UnauthorizedResponse(string message = "Invalid or missing user authentication")
    {
        return StandardError(401, message);
    }

    private IActionResult ForbiddenResponse(string message)
    {
        return StandardError(403, message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        // Get user by email
        var user = await _userRepositorie.GetUserByEmail(request.Email);
        if (user == null)
            return UnauthorizedResponse();

        // Verify password
        var isPasswordValid = _passwordHashService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
            return UnauthorizedResponse("Invalid credentials");

        // Check if user is active
        if (user.Status != Models.DB.boolStatus.True)
            return UnauthorizedResponse("User account is inactive");

        // Generate JWT token
        var token = await _jwtService.GenerateToken(user.Id, user.Role.ToString());

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

        return StandardSuccess(200, "Login successful");
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // Remove JWT cookie
        Response.Cookies.Delete("jwt");

        return StandardSuccess(200, "Logout successful");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        // Get user ID from claims
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return UnauthorizedResponse();

        // Get user from database
        var user = await _userRepositorie.GetUserById(userId);

        if (user == null)
            return UnauthorizedResponse("User not found");

        return StandardSuccess(200, "User information retrieved successfully", new
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
            user.UpdatedAt
        });
    }
}
