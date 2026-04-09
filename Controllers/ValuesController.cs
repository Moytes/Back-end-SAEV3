using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/cie")]
[Produces("application/json")]
[Authorize]
public class CIEController(
    ICIERepositorie cieRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly ICIERepositorie _cieRepositorie = cieRepositorie;
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

    [HttpGet("catalogos/dimensiones")]
    public async Task<IActionResult> GetDimensionCatalog()
    {
        var result = await _cieRepositorie.GetDimensionCatalog();
        return StandardSuccess(200, "CIE dimensions catalog retrieved successfully", result);
    }

    [HttpGet("evaluaciones")]
    public async Task<IActionResult> GetEvaluations(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null,
        [FromQuery] Guid? dimensionId = null)
    {
        var result = await _cieRepositorie.GetEvaluations(studentId, schoolYearId, dimensionId);
        return StandardSuccess(200, "CIE evaluations retrieved successfully", result);
    }

    [HttpPost("evaluaciones")]
    public async Task<IActionResult> CreateEvaluation([FromBody] AddCIEEvaluationRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _cieRepositorie.CreateEvaluation(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == CIEErrors.DimensionNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == CIEErrors.EvaluationAlreadyExistsForStudentSchoolYearAndDimension.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "cie_evaluation",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "CIE evaluation created successfully", result.Value);
    }

    [HttpPost("evaluaciones/{id:guid}/respuestas")]
    public async Task<IActionResult> UpsertAnswers(Guid id, [FromBody] UpsertCIEAnswersRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _cieRepositorie.UpsertAnswers(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == CIEErrors.EvaluationNotFound.Code ||
                result.error.Code == CIEErrors.SubIndicatorNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == CIEErrors.ReviewedEvaluationCannotBeEdited.Code ||
                result.error.Code == CIEErrors.DuplicateSubIndicatorsInRequest.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "cie_answer",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                EvaluationId = id,
                request.Items,
                request.Status
            })
        });

        return StandardSuccess(200, "CIE answers upserted successfully", result.Value);
    }

    [HttpPost("evaluaciones/{id:guid}/fonoarticulador")]
    public async Task<IActionResult> UpsertPhonologyAnswers(Guid id, [FromBody] UpsertCIEPhonologyAnswersRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _cieRepositorie.UpsertPhonologyAnswers(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == CIEErrors.EvaluationNotFound.Code ||
                result.error.Code == CIEErrors.SubIndicatorNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == CIEErrors.ReviewedEvaluationCannotBeEdited.Code ||
                result.error.Code == CIEErrors.DuplicateSubIndicatorsInRequest.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "cie_phonology_answer",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                EvaluationId = id,
                request.Items,
                request.Status
            })
        });

        return StandardSuccess(200, "CIE phonology answers upserted successfully", result.Value);
    }
}