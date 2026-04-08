using Models.DB;

namespace Models.Dto;

public class StudentDisabilityItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid DisabilityId { get; set; }
    public string DisabilityCVE { get; set; } = null!;
    public string DisabilityName { get; set; } = null!;
    public disabilitiesCategory DisabilityCategory { get; set; }
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public boolStatus ExternalDiagnosis { get; set; }
    public string? FileUrl { get; set; }
    public string? Notes { get; set; }
}