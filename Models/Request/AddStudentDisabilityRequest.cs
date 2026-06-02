using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddStudentDisabilityRequest
{
    [Required]
    public Guid DisabilityId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public BoolStatus ExternalDiagnosis { get; set; } = BoolStatus.False;
    public string? FileUrl { get; set; }
    public string? Notes { get; set; }
}