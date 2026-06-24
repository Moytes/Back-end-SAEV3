namespace Models.DB;

public class Group
{
    public int Id { get; set; }
    public string Section { get; set; } = "A";

    public int SchoolId { get; set; }
    public School School { get; set; } = null!;

    public int GradeId { get; set; }
    public Grade Grade { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
