using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;

namespace Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(
    IUserRepositorie userRepository,
    IServiceRepositorie serviceRepositorie,
    IPasswordHashService passwordHashService) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

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

    [HttpPost ("User")]
    public async Task<IActionResult> CreateUser([FromBody] AddUserRequest request)
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Hash password using PBKDF2
        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);

        // Create user
        var user = await _userRepository.CreateUser(request, passwordSalt, passwordHash);

        //string role = request.Role.ToString();

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "CREATE",
            AffectedTable = "user",
            RecordId = user.Value.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "User created successfully", user.Value);
    }
}
