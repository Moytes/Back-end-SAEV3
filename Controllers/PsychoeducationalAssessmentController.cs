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
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssessment([FromBody] AddPsychoeducationalAssessmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.CreateAssessment(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentAlreadyExistsForStudentAndSchoolYear.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "psychoeducational_assessment",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAssessment(Guid id, [FromBody] UpdatePsychoeducationalAssessmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.UpdateAssessment(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPDATE",
            AffectedTable = "psychoeducational_assessment",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return Ok(new { id });
    }

    [HttpPost("{id:guid}/bap")]
    public async Task<IActionResult> SyncBaps(Guid id, [FromBody] ManagePsychoBapsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.SyncBaps(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
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

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/colaboradores")]
    public async Task<IActionResult> SyncCollaborators(Guid id, [FromBody] ManagePsychoCollaboratorsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _psychoeducationalAssessmentRepositorie.SyncCollaborators(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == PsychoeducationalAssessmentErrors.AssessmentNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited.Code)
                return Conflict(result.error.Message);

            if (result.error.Code == PsychoeducationalAssessmentErrors.ExternalCollaboratorNameRequired.Code)
                return BadRequest(result.error.Message);

            return BadRequest(result.error.Message);
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

        return Ok(result.Value);
    }
}