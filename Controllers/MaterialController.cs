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

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet("catalogos/tipos-material")]
    public async Task<IActionResult> GetMaterialTypes()
    {
        var result = await _materialRepositorie.GetMaterialTypes();
        return Ok(result);
    }

    [HttpGet("materiales")]
    public async Task<IActionResult> GetMaterials(
        [FromQuery] string? tag = null,
        [FromQuery] Guid? dimensionId = null,
        [FromQuery] short? grade = null)
    {
        var result = await _materialRepositorie.GetMaterials(tag, dimensionId, grade);
        return Ok(result);
    }

    [HttpPost("materiales")]
    public async Task<IActionResult> CreateMaterial([FromBody] AddMaterialRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _materialRepositorie.CreateMaterial(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == MaterialErrors.MaterialTypeNotFound.Code ||
                result.error.Code == CIEErrors.DimensionNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new Models.DB.AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "material",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }

    [HttpPost("materiales/{id:guid}/tags")]
    public async Task<IActionResult> AssignTags(Guid id, [FromBody] AssignMaterialTagsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _materialRepositorie.AssignTags(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
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

        return Ok(result.Value);
    }

    [HttpGet("dialogos")]
    public async Task<IActionResult> GetDialogs([FromQuery] Guid? materialId = null)
    {
        var result = await _materialRepositorie.GetDialogs(materialId);
        return Ok(result);
    }

    [HttpPost("dialogos")]
    public async Task<IActionResult> CreateDialog([FromBody] AddDialogRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _materialRepositorie.CreateDialog(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == MaterialErrors.MaterialMustBeDialogType.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new Models.DB.AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "dialog",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }
}