using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddCanalizationRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    [Required]
    public Guid AttentionAreaId { get; set; }

    [Required]
    public Guid RequesterId { get; set; }

    [Required]
    public Guid ReceiverId { get; set; }

    public DateOnly? CanalizationDate { get; set; }

    [Required]
    public string Reason { get; set; } = null!;

    [Required]
    public string ClassroomActions { get; set; } = null!;

    public DateOnly? ReceivedDate { get; set; }
}