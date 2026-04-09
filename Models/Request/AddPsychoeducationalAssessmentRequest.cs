using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddPsychoeducationalAssessmentRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public DateOnly? EvaluationDate { get; set; }
}