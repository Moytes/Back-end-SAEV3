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

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet("indicadores")]
    public async Task<IActionResult> GetIndicators()
    {
        var result = await _teaRepositorie.GetIndicators();
        return StandardSuccess(200, "TEA indicators retrieved successfully", result);
    }

    [HttpGet("screenings")]
    public async Task<IActionResult> GetScreenings(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null,
        [FromQuery] alertLevel? alertLevel = null)
    {
        var result = await _teaRepositorie.GetScreenings(studentId, schoolYearId, alertLevel);
        return StandardSuccess(200, "TEA screenings retrieved successfully", result);
    }

    [HttpPost("screenings")]
    public async Task<IActionResult> CreateScreening([FromBody] AddTEAScreeningRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _teaRepositorie.CreateScreening(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "tea_screening",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "TEA screening created successfully", result.Value);
    }

    [HttpPost("screenings/{id:guid}/respuestas")]
    public async Task<IActionResult> UpsertAnswers(Guid id, [FromBody] UpsertTEAAnswersRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _teaRepositorie.UpsertAnswers(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == TEAErrors.ScreeningNotFound.Code ||
                result.error.Code == TEAErrors.IndicatorNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == TEAErrors.DuplicateIndicatorsInRequest.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
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

        return StandardSuccess(200, "TEA answers upserted successfully", result.Value);
    }
}