using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/area-estudiante")]
[Produces("application/json")]
[Authorize(Roles = "STUDENT")]
public class StudentAreaController(
    IAssignmentRepositorie assignmentRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IAssignmentRepositorie _assignmentRepositorie = assignmentRepositorie;
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

    private Guid? GetStudentId()
    {
        var studentIdClaim = User.FindFirst("studentId")?.Value;
        return Guid.TryParse(studentIdClaim, out var studentId) ? studentId : null;
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet("mis-asignaciones")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var studentId = GetStudentId();
        if (studentId == null)
            return StandardError(401, "User is not linked to a student");

        var result = await _assignmentRepositorie.GetAssignmentsByStudent(studentId.Value);
        return StandardSuccess(200, "Your assignments retrieved successfully", result);
    }

    [HttpPost("mis-asignaciones/{id:guid}/entregar")]
    public async Task<IActionResult> SubmitAssignment(Guid id, [FromBody] StudentSubmitAssignmentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var studentId = GetStudentId();
        if (studentId == null)
            return StandardError(401, "User is not linked to a student");

        var assignmentStudent = await _assignmentRepositorie.GetAssignmentStudentById(id);
        if (assignmentStudent == null)
            return StandardError(404, AssignmentErrors.AssignmentStudentNotFound.Message);

        if (assignmentStudent.StudentId != studentId.Value)
            return StandardError(403, "This assignment does not belong to you");

        var completeRequest = new CompleteAssignmentStudentRequest
        {
            StudentResponseJson = request.StudentResponseJson,
            ProgressPercent = 100
        };

        var result = await _assignmentRepositorie.CompleteAssignmentStudent(id, completeRequest);

        if (!result.IsSuccess)
            return StandardError(400, result.error.Message);

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetUserId(),
            Action = "SUBMIT",
            AffectedTable = "assignment_student",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "Assignment submitted successfully", new { id });
    }

    [HttpPost("dialogos/{id:guid}/progreso")]
    public async Task<IActionResult> AddDialogProgress(Guid id, [FromBody] StudentAddDialogProgressRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var studentId = GetStudentId();
        if (studentId == null)
            return StandardError(401, "User is not linked to a student");

        var addProgressRequest = new AddDialogProgressRequest
        {
            StudentId = studentId.Value,
            AssignmentStudentId = request.AssignmentStudentId,
            CurrentScene = request.CurrentScene,
            ChosenOption = request.ChosenOption,
            EmotionDetected = request.EmotionDetected,
            VoiceText = request.VoiceText,
            Completed = request.Completed
        };

        var result = await _assignmentRepositorie.AddDialogProgress(id, addProgressRequest);

        if (!result.IsSuccess)
        {
            if (result.error.Code == DialogErrors.DialogNotFound.Code ||
                result.error.Code == AssignmentErrors.AssignmentStudentNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == DialogErrors.AssignmentStudentDoesNotBelongToStudent.Code ||
                result.error.Code == DialogErrors.DialogDoesNotBelongToAssignmentMaterial.Code)
                return StandardError(403, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetUserId(),
            Action = "PROGRESS",
            AffectedTable = "dialog_progress",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Dialog progress registered successfully", result.Value);
    }
}
