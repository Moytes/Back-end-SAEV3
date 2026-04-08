using Models.DB;

namespace Models.Dto;

public class StudentRegistrationItemDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public Guid SchoolId { get; set; }
    public string SchoolName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public DateOnly IngressDate { get; set; }
    public boolStatus ItsNew { get; set; }
    public boolStatus ItsTracking { get; set; }
    public finalSituation FinalSituation { get; set; }
    public string? Notes { get; set; }
}