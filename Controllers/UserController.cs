using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Dto;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
[Authorize]
public class UserController(
    IUserRepositorie userRepository,
    IServiceRepositorie serviceRepositorie,
    IPasswordHashService passwordHashService) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

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

    [HttpGet("/api/roles")]
    public IActionResult GetRoles()
    {
        var roles = Enum.GetValues<UserRole>()
            .Select(r => new EnumOptionDto
            {
                Key = (int)r,
                Value = r.ToString()
            });

        return StandardSuccess(200, "Roles retrieved successfully", roles);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserRole? role,
        [FromQuery] Guid? schoolZoneId,
        [FromQuery] Guid? schoolId)
    {
        var users = await _userRepository.GetUsers(role, schoolZoneId, schoolId);
        return StandardSuccess(200, "Users retrieved successfully", users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] AddUserRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);

        var user = await _userRepository.CreateUser(request, passwordSalt, passwordHash);

        if (!user.IsSuccess)
            return StandardError(400, user.error.Message);

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "CREATE",
            AffectedTable = "user",
            RecordId = user.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "User created successfully", user.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _userRepository.UpdateUser(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == UserErrors.EmailAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "UPDATE",
            AffectedTable = "user",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "User updated successfully", new { id });
    }

    [HttpPost("{id:guid}/grupos")]
    public async Task<IActionResult> AssignUserToGroup(Guid id, [FromBody] AssignUserGroupRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _userRepository.AssignUserToGroup(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == GroupErrors.GroupNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == UserErrors.UserGroupAssignmentAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "CREATE",
            AffectedTable = "user_group",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                UserId = id,
                request.GroupId,
                request.SchoolYearId,
                request.EsTitular
            })
        });

        return StandardSuccess(201, "User assigned to group successfully", result.Value);
    }

    [HttpPost("{id:guid}/escuelas")]
    public async Task<IActionResult> AssignUserToSchool(Guid id, [FromBody] AssignUserSchoolRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _userRepository.AssignUserToSchool(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == UserErrors.UserSchoolAssignmentAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "CREATE",
            AffectedTable = "user_school",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                UserId = id,
                request.SchoolId,
                request.SchoolYearId
            })
        });

        return StandardSuccess(201, "User assigned to school successfully", result.Value);
    }
}