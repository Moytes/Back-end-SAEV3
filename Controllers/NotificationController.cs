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
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return StandardError(401, "Invalid or missing user authentication");

        var result = await _notificationRepositorie.GetNotificationsByUser(userId.Value);
        return StandardSuccess(200, "Notifications retrieved successfully", result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] AddNotificationRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _notificationRepositorie.CreateNotification(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == UserErrors.UserNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "notification",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Notification created successfully", result.Value);
    }

    [HttpPost("{id:guid}/leer")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _notificationRepositorie.MarkNotificationAsRead(id);

        if (!result.IsSuccess)
        {
            if (result.error.Code == NotificationErrors.NotificationNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "MARK_READ",
            AffectedTable = "notification",
            RecordId = id.ToString(),
            Request = "{}"
        });

        return StandardSuccess(200, "Notification marked as read", new { id });
    }
}