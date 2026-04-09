using Models.DB;

namespace Models.Dto;

public class NotificationItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public notificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string? Message { get; set; }
    public boolStatus Read { get; set; }
    public string? DestinationUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}