using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/notificaciones")]
[Produces("application/json")]
[Authorize]
public class NotificationController(
    INotificationRepositorie notificationRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly INotificationRepositorie _notificationRepositorie = notificationRepositorie;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized("Invalid or missing user authentication");

        var result = await _notificationRepositorie.GetNotificationsByUser(userId.Value);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] AddNotificationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _notificationRepositorie.CreateNotification(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "notification",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, result.Value);
    }

    [HttpPost("{id:guid}/leer")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _notificationRepositorie.MarkNotificationAsRead(id);

        if (!result.IsSuccess)
        {
            if (result.error.Code == NotificationErrors.NotificationNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "MARK_READ",
            AffectedTable = "notification",
            RecordId = id.ToString(),
            Request = "{}"
        });

        return Ok(new { id });
    }
}