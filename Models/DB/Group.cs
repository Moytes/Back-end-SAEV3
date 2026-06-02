namespace Models.DB;

public class Group
{
    public Guid Id { get; set; }
    public Grades Grade { get; set; }
    public string Section { get; set; } = "A";

    // Relation with School
    public School School { get; set; } = null!;
    public Guid SchoolId { get; set; }

    // Relation with school year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
