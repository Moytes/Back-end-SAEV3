using Models.DB;

namespace Models.Dto;

public class StudentRegistrationItemDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int SchoolId { get; set; }
    public string SchoolName { get; set; } = null!;
    public int SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public DateOnly IngressDate { get; set; }
    public bool IsNew { get; set; }
    public bool IsTracking { get; set; }
    public FinalSituation? FinalSituation { get; set; }
    public string? Notes { get; set; }
}
