namespace Models.DB;

public enum phases
{
    INICIAL = 1,
    FINAL = 2
}

public enum attentionTypes
{
    PLAN_INDIVIDUAL = 1,
    PLAN_ESCUELA = 2
}

public class AttentionMode
{
    public Guid Id { get; set; }
    public phases Phase { get; set; }
    public attentionTypes Type { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
