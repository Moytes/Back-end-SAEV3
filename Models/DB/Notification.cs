namespace Models.DB;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Message { get; set; }
    public NotificationType Type { get; set; } = NotificationType.INFO;
    public bool Read { get; set; }
    public string? DestinationUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
