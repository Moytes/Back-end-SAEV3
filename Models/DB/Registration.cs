namespace Models.DB;

public class Registration
{
    public int Id { get; set; }
    public DateOnly IngressDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsNew { get; set; }
    public bool IsTracking { get; set; }
    public FinalSituation? FinalSituation { get; set; }
    public string? Notes { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
