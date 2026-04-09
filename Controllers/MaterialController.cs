using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
[Authorize]
public class MaterialController(
    IMaterialRepositorie materialRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IMaterialRepositorie _materialRepositorie = materialRepositorie;
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

    [HttpGet("catalogos/tipos-material")]
    public async Task<IActionResult> GetMaterialTypes()
    {
        var result = await _materialRepositorie.GetMaterialTypes();
        return StandardSuccess(200, "Material types retrieved successfully", result);
    }

    [HttpGet("materiales")]
    public async Task<IActionResult> GetMaterials(
        [FromQuery] string? tag = null,
        [FromQuery] Guid? dimensionId = null,
        [FromQuery] short? grade = null)
    {
        var result = await _materialRepositorie.GetMaterials(tag, dimensionId, grade);
        return StandardSuccess(200, "Materials retrieved successfully", result);
    }

    [HttpPost("materiales")]
    public async Task<IActionResult> CreateMaterial([FromBody] AddMaterialRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _materialRepositorie.CreateMaterial(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == MaterialErrors.MaterialTypeNotFound.Code ||
                result.error.Code == CIEErrors.DimensionNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new Models.DB.AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "material",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Material created successfully", result.Value);
    }

    [HttpPost("materiales/{id:guid}/tags")]
    public async Task<IActionResult> AssignTags(Guid id, [FromBody] AssignMaterialTagsRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _materialRepositorie.AssignTags(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new Models.DB.AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPSERT",
            AffectedTable = "material_tag",
            RecordId = string.Join(",", result.Value!),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                MaterialId = id,
                request.Tags
            })
        });

        return StandardSuccess(200, "Material tags assigned successfully", result.Value);
    }

    [HttpGet("dialogos")]
    public async Task<IActionResult> GetDialogs([FromQuery] Guid? materialId = null)
    {
        var result = await _materialRepositorie.GetDialogs(materialId);
        return StandardSuccess(200, "Dialogs retrieved successfully", result);
    }

    [HttpPost("dialogos")]
    public async Task<IActionResult> CreateDialog([FromBody] AddDialogRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _materialRepositorie.CreateDialog(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == MaterialErrors.MaterialMustBeDialogType.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new Models.DB.AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "dialog",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Dialog created successfully", result.Value);
    }
}