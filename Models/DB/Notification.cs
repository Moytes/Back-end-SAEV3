namespace Models.DB;

public enum notificationType
{
    INFO = 0,
    WARNING = 1,
    ERROR = 2,
    SUCCESS = 3
}

public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Message { get; set; }
    public notificationType Type { get; set; } = notificationType.INFO;
    public boolStatus Read { get; set; } = boolStatus.False;
    public string? DestinationUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
}
