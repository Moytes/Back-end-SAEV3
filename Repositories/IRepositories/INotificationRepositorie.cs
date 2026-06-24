using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface INotificationRepositorie
{
    Task<IEnumerable<NotificationItemDto>> GetNotificationsByUser(Guid userId);
    Task<Result<int>> CreateNotification(AddNotificationRequest request);
    Task<Result<bool>> MarkNotificationAsRead(int notificationId);
    Task<Notification?> GetNotificationById(int notificationId);
}
