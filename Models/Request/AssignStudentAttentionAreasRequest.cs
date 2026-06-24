using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignStudentAttentionAreasRequest
{
    [Required]
    public int SchoolYearId { get; set; }

    [Required]
    [MinLength(1)]
    public List<StudentAttentionAreaAssignmentItemRequest> Areas { get; set; } = [];
}
