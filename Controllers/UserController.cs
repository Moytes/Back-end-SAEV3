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

    [HttpGet("/api/roles")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public IActionResult GetRoles()
    {
        var roles = Enum.GetValues<UserRole>()
            .Select(r => new EnumOptionDto
            {
                Key = (int)r,
                Value = r.ToString()
            });

        return Ok(roles);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserRole? role,
        [FromQuery] Guid? schoolZoneId,
        [FromQuery] Guid? schoolId)
    {
        var users = await _userRepository.GetUsers(role, schoolZoneId, schoolId);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] AddUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);

        var user = await _userRepository.CreateUser(request, passwordSalt, passwordHash);

        if (!user.IsSuccess)
        {
            if (user.error.Code == UserErrors.EmailAlreadyExists.Code ||
                user.error.Code == UserErrors.StudentAlreadyHasUserAccount.Code ||
                user.error.Code == UserErrors.StudentIdRequiredForStudentRole.Code)
                return Conflict(user.error.Message);

            if (user.error.Code == StudentErrors.StudentNotFound.Code ||
                user.error.Code == SchoolErrors.SchoolZoneNotFound.Code)
                return NotFound(user.error.Message);

            return BadRequest(user.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "CREATE",
            AffectedTable = "user",
            RecordId = user.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, user.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _userRepository.UpdateUser(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolZoneNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == UserErrors.EmailAlreadyExists.Code ||
                result.error.Code == UserErrors.StudentAlreadyHasUserAccount.Code ||
                result.error.Code == UserErrors.StudentIdRequiredForStudentRole.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            Action = "UPDATE",
            AffectedTable = "user",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return Ok(new { id });
    }

    [HttpPost("{id:guid}/grupos")]
    public async Task<IActionResult> AssignUserToGroup(Guid id, [FromBody] AssignUserGroupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _userRepository.AssignUserToGroup(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == GroupErrors.GroupNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == UserErrors.UserGroupAssignmentAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
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

        return StatusCode(201, result.Value);
    }

    [HttpPost("{id:guid}/escuelas")]
    public async Task<IActionResult> AssignUserToSchool(Guid id, [FromBody] AssignUserSchoolRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _userRepository.AssignUserToSchool(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == UserErrors.UserSchoolAssignmentAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
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

        return StatusCode(201, result.Value);
    }
}
