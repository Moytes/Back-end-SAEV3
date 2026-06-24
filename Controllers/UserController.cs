using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
[Authorize]
public class UserController(
    IUserRepositorie userRepository,
    IAdminCatalogRepositorie adminCatalogRepositorie,
    IPasswordHashService passwordHashService) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IAdminCatalogRepositorie _adminCatalogRepositorie = adminCatalogRepositorie;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    [HttpGet("/api/roles")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _adminCatalogRepositorie.GetRoles();
        return Ok(roles);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int? roleId,
        [FromQuery] int? schoolZoneId,
        [FromQuery] int? schoolId)
    {
        var users = await _userRepository.GetUsers(roleId, schoolZoneId, schoolId);
        return Ok(users);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] AddUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        if (request.RoleId != 2)
            return BadRequest("El administrador de plataforma solo puede crear usuarios SUPERVISOR.");

        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);

        var user = await _userRepository.CreateUser(request, passwordSalt, passwordHash);

        if (!user.IsSuccess)
        {
            if (user.error.Code == UserErrors.EmailAlreadyExists.Code)
                return Conflict(user.error.Message);

            if (user.error.Code == UserErrors.RoleNotFound.Code ||
                user.error.Code == SchoolErrors.SchoolZoneNotFound.Code)
                return NotFound(user.error.Message);

            return BadRequest(user.error.Message);
        }

        return StatusCode(201, user.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _userRepository.UpdateUser(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == UserErrors.RoleNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolZoneNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == UserErrors.EmailAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return Ok(new { id });
    }

    [HttpPost("{id:guid}/grupos")]
    [Authorize(Roles = "ADMIN")]
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

        return StatusCode(201, result.Value);
    }

    [HttpPost("{id:guid}/escuelas")]
    [Authorize(Roles = "ADMIN")]
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

        return StatusCode(201, result.Value);
    }

    [HttpPost("{id:guid}/supervisor-escuela")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AssignSupervisorToSchool(Guid id, [FromBody] AssignSupervisorSchoolRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _userRepository.AssignSupervisorToSchool(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }
}
