using Models.DB;

namespace Models.Dto;

public class PsychoeducationalAssessmentListItemDto
{
    public Guid Id { get; set; }
    public DateOnly EvaluationDate { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public assessmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}