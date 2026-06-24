using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/supervisor")]
[Produces("application/json")]
[Authorize(Roles = "SUPERVISOR")]
public class SupervisorController(
    IUserRepositorie userRepository,
    IPasswordHashService passwordHashService) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools()
    {
        var supervisorId = GetCurrentUserId();
        if (supervisorId == null)
            return Unauthorized();

        var schools = await _userRepository.GetSupervisorSchools(supervisorId.Value);
        return Ok(schools);
    }

    [HttpGet("personal")]
    public async Task<IActionResult> GetStaff([FromQuery] int? roleId, [FromQuery] int? schoolId)
    {
        var supervisorId = GetCurrentUserId();
        if (supervisorId == null)
            return Unauthorized();

        var staff = await _userRepository.GetSupervisorStaff(supervisorId.Value, roleId, schoolId);
        return Ok(staff);
    }

    [HttpPost("personal")]
    public async Task<IActionResult> CreateStaff([FromBody] SupervisorCreateStaffRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var supervisorId = GetCurrentUserId();
        if (supervisorId == null)
            return Unauthorized();

        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);
        var result = await _userRepository.CreateSupervisorStaff(
            supervisorId.Value,
            request,
            passwordSalt,
            passwordHash);

        if (!result.IsSuccess)
            return MapUserResultError(result.error);

        return StatusCode(201, result.Value);
    }

    [HttpPut("personal/{id:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] SupervisorUpdateStaffRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var supervisorId = GetCurrentUserId();
        if (supervisorId == null)
            return Unauthorized();

        var result = await _userRepository.UpdateSupervisorStaff(supervisorId.Value, id, request);

        if (!result.IsSuccess)
            return MapUserResultError(result.error);

        return Ok(new { id });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private IActionResult MapUserResultError(Utilities.Abstractions.Error error)
    {
        if (error.Code == UserErrors.EmailAlreadyExists.Code)
            return Conflict(error.Message);

        if (error.Code == UserErrors.UserNotFound.Code ||
            error.Code == UserErrors.RoleNotFound.Code ||
            error.Code == SchoolErrors.SchoolNotFound.Code ||
            error.Code == SchoolErrors.SchoolYearNotFound.Code)
            return NotFound(error.Message);

        if (error.Code == UserErrors.RoleNotAllowed.Code)
            return Forbid();

        return BadRequest(error.Message);
    }
}
