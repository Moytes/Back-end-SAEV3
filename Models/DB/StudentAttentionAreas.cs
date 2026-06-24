namespace Models.DB;

public class StudentAttentionAreas
{
    public int Id { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? Notes { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int AttentionAreaId { get; set; }
    public AttentionArea AttentionArea { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
