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
public class AssignmentController(
    IAssignmentRepositorie assignmentRepositorie,
    IStudentRepositorie studentRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IAssignmentRepositorie _assignmentRepositorie = assignmentRepositorie;
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

    [HttpPost("asignaciones")]
    public async Task<IActionResult> CreateAssignment([FromBody] AddAssignmentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _assignmentRepositorie.CreateAssignment(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == GroupErrors.GroupNotFound.Code ||
                result.error.Code == StudentErrors.StudentNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "assignment",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Assignment created successfully", result.Value);
    }

    [HttpGet("alumnos/{id:guid}/asignaciones")]
    public async Task<IActionResult> GetAssignmentsByStudent(Guid id)
    {
        var student = await _studentRepositorie.GetStudentById(id);
        if (student == null)
            return StandardError(404, StudentErrors.StudentNotFound.Message);

        var result = await _assignmentRepositorie.GetAssignmentsByStudent(id);
        return StandardSuccess(200, "Student assignments retrieved successfully", result);
    }

    [HttpPost("asignaciones-alumnos/{id:guid}/completar")]
    public async Task<IActionResult> CompleteAssignmentStudent(Guid id, [FromBody] CompleteAssignmentStudentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _assignmentRepositorie.CompleteAssignmentStudent(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == AssignmentErrors.AssignmentStudentNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "COMPLETE",
            AffectedTable = "assignment_student",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "Assignment student completed successfully", new { id });
    }

    [HttpPost("dialogos/{id:guid}/progreso")]
    public async Task<IActionResult> AddDialogProgress(Guid id, [FromBody] AddDialogProgressRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _assignmentRepositorie.AddDialogProgress(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == DialogErrors.DialogNotFound.Code ||
                result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == AssignmentErrors.AssignmentStudentNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == DialogErrors.AssignmentStudentDoesNotBelongToStudent.Code ||
                result.error.Code == DialogErrors.DialogDoesNotBelongToAssignmentMaterial.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "dialog_progress",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                DialogId = id,
                request.StudentId,
                request.AssignmentStudentId,
                request.CurrentScene,
                request.ChosenOption,
                request.EmotionDetected,
                request.VoiceText,
                request.Completed
            })
        });

        return StandardSuccess(201, "Dialog progress created successfully", result.Value);
    }
}