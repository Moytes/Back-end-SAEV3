using Models.DB;

namespace Models.Dto;

public class TEAScreeningListItemDto
{
    public Guid Id { get; set; }
    public DateOnly ScreeningDate { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid EvaluatorId { get; set; }
    public string EvaluatorFullName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public string? ObservationContext { get; set; }
    public string? GeneralObservations { get; set; }
    public short? TotalScore { get; set; }
    public alertLevel? AlertLevel { get; set; }
    public boolStatus RequiresCanalization { get; set; }
    public DateTime CreatedAt { get; set; }
}