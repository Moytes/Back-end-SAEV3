using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddStudentDisabilityRequest
{
    [Required]
    public int DisabilityId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public bool ExternalDiagnosis { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Notes { get; set; }
}
