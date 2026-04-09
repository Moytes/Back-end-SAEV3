using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddCIEEvaluationRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid EvaluatorId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    [Required]
    public Guid DimensionId { get; set; }

    public DateOnly? EvaluationDate { get; set; }
    public string? Observations { get; set; }
}