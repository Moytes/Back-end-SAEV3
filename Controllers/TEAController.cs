using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/tea")]
[Produces("application/json")]
[Authorize]
public class TEAController(
    ITEARepositorie teaRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly ITEARepositorie _teaRepositorie = teaRepositorie;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet("indicadores")]
    public async Task<IActionResult> GetIndicators()
    {
        var result = await _teaRepositorie.GetIndicators();
        return Ok(result);
    }

    [HttpGet("screenings")]
    public async Task<IActionResult> GetScreenings(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null,
        [FromQuery] alertLevel? alertLevel = null)
    {
        var result = await _teaRepositorie.GetScreenings(studentId, schoolYearId, alertLevel);
        return Ok(result);
    }

    [HttpPost("screenings")]
    public async Task<IActionResult> CreateScreening([FromBody] AddTEAScreeningRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _teaRepositorie.CreateScreening(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "tea_screening",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }

    [HttpPost("screenings/{id:guid}/respuestas")]
    public async Task<IActionResult> UpsertAnswers(Guid id, [FromBody] UpsertTEAAnswersRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _teaRepositorie.UpsertAnswers(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == TEAErrors.ScreeningNotFound.Code ||
                result.error.Code == TEAErrors.IndicatorNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == TEAErrors.DuplicateIndicatorsInRequest.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "tea_answer",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                ScreeningId = id,
                request.Items
            })
        });

        return Ok(result.Value);
    }
}
