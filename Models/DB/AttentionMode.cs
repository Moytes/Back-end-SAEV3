namespace Models.DB;

public enum Phases
{
    INICIAL = 1,
    FINAL = 2
}

public enum AttentionTypes
{
    PLAN_INDIVIDUAL = 1,
    PLAN_ESCUELA = 2
}

public class AttentionMode
{
    public Guid Id { get; set; }
    public Phases Phase { get; set; }
    public AttentionTypes Type { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
