using Models.DB;

namespace Models.Dto;

public class TEAAlertItemDto
{
    public Guid ScreeningId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public DateOnly ScreeningDate { get; set; }
    public short? TotalScore { get; set; }
    public alertLevel? AlertLevel { get; set; }
    public boolStatus RequiresCanalization { get; set; }
}