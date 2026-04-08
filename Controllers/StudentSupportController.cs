using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
[Authorize]
public class StudentSupportController(
    IStudentSupportRepositorie studentSupportRepositorie,
    IStudentRepositorie studentRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IStudentSupportRepositorie _studentSupportRepositorie = studentSupportRepositorie;
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
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

    [HttpGet("catalogos/discapacidades")]
    public async Task<IActionResult> GetDisabilityCatalog()
    {
        var result = await _studentSupportRepositorie.GetDisabilityCatalog();
        return StandardSuccess(200, "Disability catalog retrieved successfully", result);
    }

    [HttpGet("catalogos/areas-atencion")]
    public async Task<IActionResult> GetAttentionAreasCatalog()
    {
        var result = await _studentSupportRepositorie.GetAttentionAreasCatalog();
        return StandardSuccess(200, "Attention areas catalog retrieved successfully", result);
    }

    [HttpGet("alumnos/{id:guid}/discapacidades")]
    public async Task<IActionResult> GetStudentDisabilities(Guid id)
    {
        var student = await _studentRepositorie.GetStudentById(id);
        if (student == null)
            return StandardError(404, StudentErrors.StudentNotFound.Message);

        var result = await _studentSupportRepositorie.GetStudentDisabilities(id);
        return StandardSuccess(200, "Student disabilities retrieved successfully", result);
    }

    [HttpPost("alumnos/{id:guid}/discapacidades")]
    public async Task<IActionResult> AddStudentDisability(Guid id, [FromBody] AddStudentDisabilityRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentSupportRepositorie.AddStudentDisability(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == DisabilityErrors.DisabilityNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == DisabilityErrors.StudentDisabilityAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "student_disability",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                StudentId = id,
                request.DisabilityId,
                request.SchoolYearId,
                request.ExternalDiagnosis,
                request.FileUrl,
                request.Notes
            })
        });

        return StandardSuccess(201, "Student disability assigned successfully", result.Value);
    }

    [HttpPost("alumnos/{id:guid}/areas-atencion")]
    public async Task<IActionResult> AssignStudentAttentionAreas(Guid id, [FromBody] AssignStudentAttentionAreasRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentSupportRepositorie.AssignStudentAttentionAreas(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == AttentionAreaErrors.AttentionAreaNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == AttentionAreaErrors.DuplicateAttentionAreasInRequest.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "student_attention_area",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                StudentId = id,
                request.SchoolYearId,
                request.Areas
            })
        });

        return StandardSuccess(200, "Student attention areas assigned successfully", result.Value);
    }

    [HttpPost("alumnos/{id:guid}/modalidad-atencion")]
    public async Task<IActionResult> AddAttentionMode(Guid id, [FromBody] AddAttentionModeRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentSupportRepositorie.AddAttentionMode(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == AttentionModeErrors.AttentionModeAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "attention_mode",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                StudentId = id,
                request.SchoolYearId,
                request.Phase,
                request.Type
            })
        });

        return StandardSuccess(201, "Attention mode registered successfully", result.Value);
    }
}
