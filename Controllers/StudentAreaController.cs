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
            return Unauthorized("User is not linked to a student");

        var result = await _assignmentRepositorie.GetAssignmentsByStudent(studentId.Value);
        return Ok(result);
    }

    [HttpPost("mis-asignaciones/{id:guid}/entregar")]
    public async Task<IActionResult> SubmitAssignment(Guid id, [FromBody] StudentSubmitAssignmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var studentId = GetStudentId();
        if (studentId == null)
            return Unauthorized("User is not linked to a student");

        var assignmentStudent = await _assignmentRepositorie.GetAssignmentStudentById(id);
        if (assignmentStudent == null)
            return NotFound(AssignmentErrors.AssignmentStudentNotFound.Message);

        if (assignmentStudent.StudentId != studentId.Value)
            return Forbid();

        var completeRequest = new CompleteAssignmentStudentRequest
        {
            StudentResponseJson = request.StudentResponseJson,
            ProgressPercent = 100
        };

        var result = await _assignmentRepositorie.CompleteAssignmentStudent(id, completeRequest);

        if (!result.IsSuccess)
            return BadRequest(result.error.Message);

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetUserId(),
            Action = "SUBMIT",
            AffectedTable = "assignment_student",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return Ok(new { id });
    }

    [HttpPost("dialogos/{id:guid}/progreso")]
    public async Task<IActionResult> AddDialogProgress(Guid id, [FromBody] StudentAddDialogProgressRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var studentId = GetStudentId();
        if (studentId == null)
            return Unauthorized("User is not linked to a student");

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
                return NotFound(result.error.Message);

            if (result.error.Code == DialogErrors.AssignmentStudentDoesNotBelongToStudent.Code ||
                result.error.Code == DialogErrors.DialogDoesNotBelongToAssignmentMaterial.Code)
                return Forbid();

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetUserId(),
            Action = "PROGRESS",
            AffectedTable = "dialog_progress",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }
}