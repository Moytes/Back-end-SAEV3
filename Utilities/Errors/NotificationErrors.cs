using Utilities.Abstractions;

namespace Utilities.Errors;

public static class NotificationErrors
{
    public static readonly Error NotificationNotFound =
        new("Notification.NotificationNotFound", "Notification not found.");
}