namespace Models.DB;

public class StudentAttentionAreas
{
    public Guid Id { get; set; }
    public boolStatus IsRequired { get; set; } = boolStatus.True;
    public string? Notes { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }
    // Attention Area
    public AttentionArea AttentionArea { get; set; } = null!;
    public Guid AttentionAreaId { get; set; }
    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
