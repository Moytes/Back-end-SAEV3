namespace Models.DB;

public class AttentionMode
{
    public int Id { get; set; }
    public Phases Phase { get; set; }
    public AttentionTypes Type { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
