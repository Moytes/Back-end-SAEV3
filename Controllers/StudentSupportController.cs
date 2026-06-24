using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
[Authorize]
public class StudentSupportController(
    IStudentSupportRepositorie studentSupportRepositorie,
    IStudentRepositorie studentRepositorie) : ControllerBase
{
    private readonly IStudentSupportRepositorie _studentSupportRepositorie = studentSupportRepositorie;
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;

    [HttpGet("catalogos/discapacidades")]
    public async Task<IActionResult> GetDisabilityCatalog()
    {
        var result = await _studentSupportRepositorie.GetDisabilityCatalog();
        return Ok(result);
    }

    [HttpGet("catalogos/areas-atencion")]
    public async Task<IActionResult> GetAttentionAreasCatalog()
    {
        var result = await _studentSupportRepositorie.GetAttentionAreasCatalog();
        return Ok(result);
    }

    [HttpGet("alumnos/{id:guid}/discapacidades")]
    public async Task<IActionResult> GetStudentDisabilities(Guid id)
    {
        var student = await _studentRepositorie.GetStudentById(id);
        if (student == null)
            return NotFound(StudentErrors.StudentNotFound.Message);

        var result = await _studentSupportRepositorie.GetStudentDisabilities(id);
        return Ok(result);
    }

    [HttpPost("alumnos/{id:guid}/discapacidades")]
    public async Task<IActionResult> AddStudentDisability(Guid id, [FromBody] AddStudentDisabilityRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentSupportRepositorie.AddStudentDisability(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == DisabilityErrors.DisabilityNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == DisabilityErrors.StudentDisabilityAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }

    [HttpPost("alumnos/{id:guid}/areas-atencion")]
    public async Task<IActionResult> AssignStudentAttentionAreas(Guid id, [FromBody] AssignStudentAttentionAreasRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentSupportRepositorie.AssignStudentAttentionAreas(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == AttentionAreaErrors.AttentionAreaNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == AttentionAreaErrors.DuplicateAttentionAreasInRequest.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("alumnos/{id:guid}/modalidad-atencion")]
    public async Task<IActionResult> AddAttentionMode(Guid id, [FromBody] AddAttentionModeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentSupportRepositorie.AddAttentionMode(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == AttentionModeErrors.AttentionModeAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }
}
