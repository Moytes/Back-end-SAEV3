using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface INotificationRepositorie
{
    Task<IEnumerable<NotificationItemDto>> GetNotificationsByUser(Guid userId);
    Task<Result<Guid>> CreateNotification(AddNotificationRequest request);
    Task<Result<bool>> MarkNotificationAsRead(Guid notificationId);
    Task<Notification?> GetNotificationById(Guid notificationId);
}