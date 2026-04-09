using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/evaluaciones-psicopedagogicas")]
[Produces("application/json")]
[Authorize]
public class PsychoeducationalAssessmentController(
    IPsychoeducationalAssessmentRepositorie psychoeducationalAssessmentRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IPsychoeducationalAssessmentRepositorie _psychoeducationalAssessmentRepositorie = psychoeducationalAssessmentRepositorie;
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

    [HttpGet]
    public async Task<IActionResult> GetAssessments(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _psychoeducationalAssessmentRepositorie.GetAssessments(studentId, schoolYearId);
        return StandardSuccess(200, "Psychoeducational assessments retrieved successfully", result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssessment([FromBody] AddPsychoeducationalAssessmentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.CreateAssessment(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentAlreadyExistsForStudentAndSchoolYear.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "psychoeducational_assessment",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Psychoeducational assessment created successfully", result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAssessment(Guid id, [FromBody] UpdatePsychoeducationalAssessmentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.UpdateAssessment(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPDATE",
            AffectedTable = "psychoeducational_assessment",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "Psychoeducational assessment updated successfully", new { id });
    }

    [HttpPost("{id:guid}/bap")]
    public async Task<IActionResult> SyncBaps(Guid id, [FromBody] ManagePsychoBapsRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.SyncBaps(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "psycho_bap",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                AssessmentId = id,
                request.Items
            })
        });

        return StandardSuccess(200, "Assessment BAP indicators synchronized successfully", result.Value);
    }

    [HttpPost("{id:guid}/colaboradores")]
    public async Task<IActionResult> SyncCollaborators(Guid id, [FromBody] ManagePsychoCollaboratorsRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.SyncCollaborators(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return StandardError(409, result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.ExternalCollaboratorNameRequired.Code)
                return StandardError(400, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "psycho_collaborator",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                AssessmentId = id,
                request.Items
            })
        });

        return StandardSuccess(200, "Assessment collaborators synchronized successfully", result.Value);
    }
}