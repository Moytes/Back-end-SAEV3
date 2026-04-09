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
    public async Task<IActionResult> GetCanalizations(
        [FromQuery] canalizationStatus? status = null,
        [FromQuery] Guid? requesterId = null,
        [FromQuery] Guid? receiverId = null)
    {
        var result = await _canalizationRepositorie.GetCanalizations(status, requesterId, receiverId);
        return StandardSuccess(200, "Canalizations retrieved successfully", result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCanalization([FromBody] AddCanalizationRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _canalizationRepositorie.CreateCanalization(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == AttentionAreaErrors.AttentionAreaNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == CanalizationErrors.RequesterAndReceiverMustBeDifferent.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "canalization",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Canalization created successfully", result.Value);
    }

    [HttpPut("{id:guid}/estado")]
    public async Task<IActionResult> UpdateCanalizationStatus(Guid id, [FromBody] UpdateCanalizationStatusRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _canalizationRepositorie.UpdateCanalizationStatus(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == CanalizationErrors.CanalizationNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == CanalizationErrors.ClosedCanalizationCannotBeReopened.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPDATE_STATUS",
            AffectedTable = "canalization",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "Canalization status updated successfully", new { id });
    }
}