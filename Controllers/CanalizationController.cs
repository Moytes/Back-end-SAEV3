using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/canalizaciones")]
[Produces("application/json")]
[Authorize]
public class CanalizationController(
    ICanalizationRepositorie canalizationRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly ICanalizationRepositorie _canalizationRepositorie = canalizationRepositorie;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetCanalizations(
        [FromQuery] canalizationStatus? status = null,
        [FromQuery] Guid? requesterId = null,
        [FromQuery] Guid? receiverId = null)
    {
        var result = await _canalizationRepositorie.GetCanalizations(status, requesterId, receiverId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCanalization([FromBody] AddCanalizationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _canalizationRepositorie.CreateCanalization(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == AttentionAreaErrors.AttentionAreaNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == CanalizationErrors.RequesterAndReceiverMustBeDifferent.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "canalization",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }

    [HttpPut("{id:guid}/estado")]
    public async Task<IActionResult> UpdateCanalizationStatus(Guid id, [FromBody] UpdateCanalizationStatusRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _canalizationRepositorie.UpdateCanalizationStatus(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == CanalizationErrors.CanalizationNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == CanalizationErrors.ClosedCanalizationCannotBeReopened.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPDATE_STATUS",
            AffectedTable = "canalization",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return Ok(new { id });
    }
}