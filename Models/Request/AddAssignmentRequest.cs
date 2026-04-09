using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddAssignmentRequest
{
    [Required]
    public Guid MaterialId { get; set; }

    [Required]
    public Guid AssignedById { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public Guid? GroupId { get; set; }

    [MinLength(1)]
    public List<Guid>? StudentIds { get; set; }

    public DateOnly? AssignedDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Instructions { get; set; }
    public string? EvaluationCriteriaJson { get; set; }
}