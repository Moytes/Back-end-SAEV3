using Models.DB;

namespace Models.Dto;

public class StudentDisabilityItemDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public int DisabilityId { get; set; }
    public string DisabilityCVE { get; set; } = null!;
    public string DisabilityName { get; set; } = null!;
    public DisabilityCategory DisabilityCategory { get; set; }
    public int SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public bool ExternalDiagnosis { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Notes { get; set; }
}
