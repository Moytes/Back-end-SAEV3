using Models.DB;

namespace Models.Dto;

public class NotificationItemDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string? Message { get; set; }
    public bool Read { get; set; }
    public string? DestinationUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
