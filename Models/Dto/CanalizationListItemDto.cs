using Models.DB;

namespace Models.Dto;

public class CanalizationListItemDto
{
    public Guid Id { get; set; }
    public DateOnly CanalizationDate { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public Guid AttentionAreaId { get; set; }
    public string AttentionAreaName { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string ClassroomActions { get; set; } = null!;
    public Guid RequesterId { get; set; }
    public string RequesterFullName { get; set; } = null!;
    public Guid ReceiverId { get; set; }
    public string ReceiverFullName { get; set; } = null!;
    public DateOnly? ReceivedDate { get; set; }
    public canalizationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}