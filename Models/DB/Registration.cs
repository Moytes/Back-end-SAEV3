namespace Models.DB;

public enum finalSituation
{
    ALTA = 1,
    BAJA = 2,
    SEGUIMIENTO = 3
}

public class Registration
{
    public Guid Id { get; set; }
    public DateOnly IngressDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public boolStatus ItsNew { get; set; } = boolStatus.False;
    public boolStatus ItsTracking { get; set; } = boolStatus.False;
    public finalSituation FinalSituation { get; set; }
    public string? Notes { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // Group
    public Group Group { get; set; } = null!;
    public Guid GroupId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
