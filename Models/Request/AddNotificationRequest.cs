using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddNotificationRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public string? Message { get; set; }
    public string? DestinationUrl { get; set; }
}
